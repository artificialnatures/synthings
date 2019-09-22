namespace synthings.core

type Identifier =
    {
        Name : string;
        Id : System.Guid
    }

type Identifier with
    static member empty = {Id = System.Guid.Empty; Name = System.String.Empty}

type MachineBuilder = System.Guid -> Machine

type Topic =
    {
        Identifier : Identifier;
        BuildMachine : MachineBuilder;
        Behaviors : Identifier list
    }

type Library =
    {
        Identifier : Identifier;
        Topics : Topic list
    }

type Library with
    static member empty = {Identifier = Identifier.empty; Topics = List.empty}

type LibraryModule =
    abstract member Library : Library

type LibraryCollection =
    {
        Libraries : Library list
    }

type LibraryCollection with
    static member empty = {Libraries = List.empty}
    static member add (library : Library) (collection : LibraryCollection) =
        {collection with Libraries = List.append collection.Libraries [library]}
    static member append (libraries : seq<Library>) (collection : LibraryCollection) =
        let merged =
            collection.Libraries
            |> Seq.append libraries
            |> Seq.toList
        {collection with Libraries = merged}

module library =
    open System.IO
    open System.Reflection
    
    type CoreTopic =
        | Basic
            | Relay
            | Error
    
    let internal identifiers =
        Map.empty
        |> Map.add CoreTopic.Basic {Id = System.Guid.Parse("03B71882-8158-40F9-8CC7-71347E3FC784"); Name = CoreTopic.Basic.ToString()}
        |> Map.add CoreTopic.Relay {Id = System.Guid.Parse("DCB280FA-ABFF-4564-A84D-14D38D9405E3"); Name = CoreTopic.Relay.ToString()}
        |> Map.add CoreTopic.Error {Id = System.Guid.Parse("7D956605-9C75-4482-9B6E-C0620A69FCFC"); Name = CoreTopic.Error.ToString()}
    
    let internal reverseLookup =
        Map.toSeq identifiers
        |> Seq.map (fun pair -> (snd pair).Id, fst pair)
        |> Map.ofSeq
    
    let internal behaviors =
        Map.empty
        |> Map.add (identifiers.Item CoreTopic.Relay).Id behavior.relay
        |> Map.add (identifiers.Item CoreTopic.Error).Id behavior.error
    
    let internal mapBehavior (behaviorId : System.Guid) =
        let identifier = reverseLookup.TryFind behaviorId
        match identifier with
        | Some identifier -> machine.createMachine (identifier.ToString()) (behaviors.Item behaviorId)
        | None -> machine.createMachine (Error.ToString()) (behaviors.Item (identifiers.Item CoreTopic.Error).Id)
    
    let internal mapTopic (topicId : System.Guid) =
        let identifier = reverseLookup.TryFind topicId
        match identifier with
        | Some identifier -> mapBehavior
        | None -> mapBehavior
    
    let internal basicTopic =
        {
            Identifier = identifiers.Item Basic;
            BuildMachine = mapTopic (identifiers.Item Basic).Id;
            Behaviors = [identifiers.Item Relay; identifiers.Item Error]
        }
    
    let internal findLibraryModule (assembly : Assembly) =
        Seq.tryFind (fun (m : Module) -> m.Name = "library") assembly.Modules
    
    let internal loadLibraryField (libraryField : FieldInfo) =
        match libraryField.GetValue() with
        | :? Library as library -> library
        | _ -> Library.empty
    
    let isLibraryModule (typeInfo : TypeInfo) =
        typeInfo.ImplementedInterfaces
        |> Seq.contains typeof<LibraryModule>
    
    let internal loadLibrary (assembly : Assembly) =
        let libraryType = Seq.tryFind isLibraryModule assembly.DefinedTypes
        let instance = assembly.CreateInstance(libraryType.Value.FullName)
        match instance with
        | :? LibraryModule as libraryModule -> libraryModule.Library
        | _ -> Library.empty
    
    let internal isSynthingsAssembly (assembly : Assembly) =
        let libraryModule = findLibraryModule assembly
        match libraryModule with
        | Some m -> (m.GetField "Library").GetType() = typeof<Library>
        | None -> false
    
    let internal hasSynthingsAssemblyName (filePath : string) =
        let filename = Path.GetFileName filePath
        Path.GetExtension filename = ".dll" &&
        filename.Contains "synthings" &&
        filename <> "synthings.core.dll" &&
        not (filename.Contains "test")
    
    let internal findAssemblies () =
        Assembly.GetAssembly(typeof<Library>).Location
        |> (fun path -> (Directory.GetParent path).FullName)
        |> Directory.EnumerateFiles
        |> Seq.filter hasSynthingsAssemblyName
        |> Seq.map (fun path -> Assembly.LoadFile path)
        |> Seq.filter isSynthingsAssembly
    
    let internal findLibraries () =
        findAssemblies ()
        |> Seq.map loadLibrary
    
    let internal mergeLibrary (first : Library) (second : Library) =
        {first with Topics = List.append first.Topics second.Topics}
    
    let internal coreLibrary =
        {
            Identifier = {Id = System.Guid.Parse("CD2476A1-1A0C-461D-8A3D-5220F935CD96"); Name = "Core"};
            Topics = [basicTopic]
        }
    
    let create () =
        LibraryCollection.empty
        |> LibraryCollection.add coreLibrary
        |> LibraryCollection.append (findLibraries ())
