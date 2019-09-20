module synthings.core.tests.Libraries

open Xunit
open System.IO
open System.Reflection
open synthings.core

[<Fact>]
let ``Create library`` () =
    let coreLibrary = library.create ()
    let firstTopic = coreLibrary.Topics.Head
    Assert.Equal(1, coreLibrary.Topics.Length)
    Assert.Equal("Basic", firstTopic.Identifier.Name)
    Assert.Equal(2, firstTopic.Behaviors.Length)

[<Fact>]
let ``Create a machine from a library`` () =
    let coreLibrary = library.create ()
    let firstTopic = coreLibrary.Topics.Head
    let firstBehavior = firstTopic.Behaviors.Head
    let machine = firstTopic.BuildMachine firstBehavior.Id
    Assert.Equal("Relay", machine.Name)

[<Fact>]
let ``Can load libraries`` () =
    let assemblies =
        Assembly.GetAssembly(typeof<Library>).Location
        |> (fun path -> (Directory.GetParent path).FullName)
        |> Directory.EnumerateFiles
        |> Seq.filter (fun path ->
            let filename = Path.GetFileName path
            Path.GetExtension filename = ".dll" &&
            filename.Contains "synthings" &&
            filename <> "synthings.core.dll" &&
            not (filename.Contains "test"))
        |> Seq.map (fun path -> Assembly.LoadFile path)
        //|> Seq.filter (fun assembly -> Seq.exists (fun (m : Module) -> m.Name = "library") assembly.Modules)
    Assert.True(Seq.length assemblies > 0)

[<Fact>]
let ``Find library module`` () =
    let scalarAssemblyPath =
        Assembly.GetAssembly(typeof<Library>).Location
        |> (fun path -> (Directory.GetParent path).FullName)
        |> (fun path -> Path.Combine (path, "synthings.scalar.dll"))
    let scalarAssembly = Assembly.LoadFile scalarAssemblyPath
    let isLibraryModule (typeInfo : TypeInfo) =
        typeInfo.ImplementedInterfaces
        |> Seq.contains typeof<LibraryModule>
    let libraryType = Seq.tryFind isLibraryModule scalarAssembly.DefinedTypes
    let instance = scalarAssembly.CreateInstance(libraryType.Value.FullName)
    let library =
        match instance with
        | :? LibraryModule as libraryModule -> libraryModule.Library
        | _ -> Library.empty
    Assert.True(library.Topics.Length > 0)
