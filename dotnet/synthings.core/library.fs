namespace synthings.core

type Identifier =
    {
        Name : string;
        Id : System.Guid
    }

type MachineBuilder = System.Guid -> Machine

type Topic =
    {
        Identifier : Identifier;
        BuildMachine : MachineBuilder;
        Behaviors : Identifier list
    }

type Library =
    {
        Topics : Topic list;
    }

type Library with
    static member empty = {Topics = List.empty}

type LibraryModule =
    abstract member Library : Library

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
        | _ -> {Topics = List.empty}
    
    let internal loadLibrary (assembly : Assembly) =
        let libraryModule = findLibraryModule assembly
        match libraryModule with
        | Some m -> loadLibraryField (m.GetField "Library")
        | None -> {Topics = List.empty}
    
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
    
    let create () =
        [{Topics = [basicTopic]}]
        |> Seq.append (findLibraries ())
        |> Seq.reduce mergeLibrary
