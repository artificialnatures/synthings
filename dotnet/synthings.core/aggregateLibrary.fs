namespace synthings.core
open System

module aggregateLibrary =
    open System.IO
    open System.Reflection
    
    let internal isLibraryResolver (typeInfo : TypeInfo) =
        Seq.exists (fun interfaceType -> interfaceType = typeof<LibraryResolver>) typeInfo.ImplementedInterfaces
    
    let internal findLibraryResolvers (assembly : Assembly) =
        Seq.filter isLibraryResolver assembly.DefinedTypes
    
    let internal instantiateLibraries (assembly : Assembly) =
        findLibraryResolvers assembly
        |> Seq.map Activator.CreateInstance
        |> Seq.map (fun obj -> obj :?> LibraryResolver)
        |> Seq.toList
    
    let internal isSynthingsAssembly (assembly : Assembly) =
        Seq.isEmpty (findLibraryResolvers assembly)
    
    let internal hasSynthingsAssemblyName (filePath : string) =
        let filename = Path.GetFileName filePath
        Path.GetExtension filename = ".dll" &&
        filename.Contains "synthings" &&
        filename <> "synthings.core.dll" &&
        not (filename.Contains "test")
    
    let internal findAssemblies () =
        let p = Assembly.GetAssembly(typeof<Signal>).Location
        let fp = (fun path -> (Directory.GetParent path).FullName) p
        let aps = Directory.EnumerateFiles fp
        let sap = Seq.filter hasSynthingsAssemblyName aps
        let assemblies = Seq.map (fun path -> Assembly.LoadFile path) sap
        Seq.toList assemblies
    
    let internal loadLibraries () =
        let assemblies = findAssemblies()
        let libs = List.map instantiateLibraries assemblies
        let ll = List.concat libs
        List.append [CoreLibrary.build()] ll
    
    let internal listTopics (libraries : seq<LibraryResolver>) =
        libraries
        |> Seq.map (fun library -> library.listTopics())
        |> Seq.concat
        |> Seq.toList
    
    type internal TopicMap =
        {
            Topic : TopicDescriptor;
            Library : LibraryResolver;
            Behaviors : BehaviorDescriptor list
        }
    
    type internal TopicMap with
        static member containsBehavior (behaviorDescriptor : BehaviorDescriptor) (topicMap : TopicMap) =
            List.exists (fun (behavior : BehaviorDescriptor) -> behavior.Id = behaviorDescriptor.Id) topicMap.Behaviors
    
    let internal createTopicMap (topicDescriptor : TopicDescriptor) (library : LibraryResolver) =
        let behaviors = library.listBehaviors topicDescriptor
        {Topic = topicDescriptor; Library = library; Behaviors = behaviors}
    
    let internal buildTopicMap (library : LibraryResolver) =
        library.listTopics()
        |> List.map (fun topicDescriptor -> createTopicMap topicDescriptor library)
    
    let internal buildAggregateTopicMap (libraries : seq<LibraryResolver>) =
        libraries
        |> Seq.map buildTopicMap
        |> List.concat
        |> List.map (fun topicMap -> (topicMap.Topic.Id, topicMap))
        |> Map.ofList
    
    type internal AggregateLibrary() =
        let _libraries = loadLibraries()
        let _topics = listTopics _libraries
        let _topicMaps = buildAggregateTopicMap _libraries
        let _listBehaviors (topicDescriptor : TopicDescriptor) = _topicMaps.Item(topicDescriptor.Id).Behaviors
        let _createMachine behaviorDescriptor =
            let matchingTopicMap =
                List.map (fun (topic : TopicDescriptor) -> topic.Id) _topics
                |> List.map (fun id -> _topicMaps.Item id)
                |> List.tryFind (fun topicMap -> TopicMap.containsBehavior behaviorDescriptor topicMap)
            match matchingTopicMap with
            | Some topicMap -> topicMap.Library.createMachine behaviorDescriptor
            | None -> Machine.createError()
        member this.Libraries = _libraries
        member this.Topics = _topics
        member this.listBehaviors topicDescriptor = _listBehaviors topicDescriptor
        member this.createMachine behaviorDescriptor = _createMachine behaviorDescriptor
        interface LibraryResolver with
            member this.Origin = "Aggregate"
            member this.Name = "Aggregate"
            member this.listTopics () = _topics
            member this.listBehaviors topicDescriptor = _listBehaviors topicDescriptor
            member this.createMachine behaviorDescriptor = library.createMachine behaviorDescriptor.Behavior
