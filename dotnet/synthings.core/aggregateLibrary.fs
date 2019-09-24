namespace synthings.core

module aggregateLibrary =
    open System.IO
    open System.Reflection
    
    let internal isLibraryResolver (typeInfo : TypeInfo) =
        typeInfo.IsAssignableFrom(typeof<LibraryResolver>)
    
    let internal typeName (typeInfo : TypeInfo) =
        typeInfo.GetType().FullName
    
    let findLibraryResolvers (assembly : Assembly) =
        Seq.filter isLibraryResolver assembly.DefinedTypes
    
    let internal instantiateLibraries (assembly : Assembly) =
        findLibraryResolvers assembly
        |> Seq.map typeName
        |> Seq.map assembly.CreateInstance
        |> Seq.map (fun obj -> obj :?> LibraryResolver)
        |> Seq.toList
    
    let isSynthingsAssembly (assembly : Assembly) =
        Seq.isEmpty (findLibraryResolvers assembly)
    
    let internal hasSynthingsAssemblyName (filePath : string) =
        let filename = Path.GetFileName filePath
        Path.GetExtension filename = ".dll" &&
        filename.Contains "synthings" &&
        filename <> "synthings.core.dll" &&
        not (filename.Contains "test")
    
    let findAssemblies () =
        Assembly.GetAssembly(typeof<Signal>).Location
        |> (fun path -> (Directory.GetParent path).FullName)
        |> Directory.EnumerateFiles
        |> Seq.filter hasSynthingsAssemblyName
        |> Seq.map (fun path -> Assembly.LoadFile path)
        |> Seq.toList
    
    let loadLibraries () =
        findAssemblies()
        |> List.map instantiateLibraries
        |> List.concat
        |> List.append [library.build()]
    
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
        member topicMap.containsBehavior (behaviorDescriptor : BehaviorDescriptor) =
            List.exists (fun behavior -> behavior.Id = behaviorDescriptor.Id) topicMap.Behaviors
    
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
    
    type internal AggregateLibrary(libraries : LibraryResolver list) =
        member internal this.Libraries = libraries
        member internal this.Topics = listTopics this.Libraries
        member internal this.TopicMap = buildAggregateTopicMap this.Libraries
        interface LibraryResolver with
            member this.listTopics () = this.Topics
            member this.listBehaviors topicDescriptor = this.TopicMap.Item(topicDescriptor.Id).Behaviors
            member this.createMachine behaviorDescriptor = 
                let topicMap =
                    List.map (fun (topic : TopicDescriptor) -> topic.Id) this.Topics
                    |> List.map (fun id -> this.TopicMap.Item id)
                    |> List.tryFind (fun topicMap -> topicMap.containsBehavior behaviorDescriptor)
                match topicMap with
                | Some topicMap -> topicMap.Library.createMachine behaviorDescriptor
                | None -> machine.createError()
    
    let build () =
        let libraries = loadLibraries()
        AggregateLibrary(libraries) :> LibraryResolver
