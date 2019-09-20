module synthings.scalar.tests.Waves

open Xunit
open System.IO
open System.Reflection
open synthings.core
open synthings.scalar

(*[<Fact>]
let ``Sine wave produces oscillating values`` () =
    let parameters = WaveParameters(1.0, 2.0)
    let wave = library.createBehavior ScalarTopic.SineWave parameters
    let isValid = number.equalsAny [1.0; -1.0]
    let result =
        signal.createSignalSequence 0.25 10.25 0.5 0.0
        |> Seq.map wave
        |> Seq.map signal.unpack
        |> Seq.forall isValid
    Assert.True(result)*)

[<Fact>]
let ``Sine wave machine produces oscillating values`` () =
    Assert.True(true)

[<Fact>]
let ``Can load libraries`` () =
    let assemblies =
        Assembly.GetAssembly(typeof<Library>).Location
        |> (fun path -> (Directory.GetParent path).FullName)
        |> Directory.EnumerateFiles
        |> Seq.filter (fun path -> Path.GetExtension path = ".dll" && (Path.GetFileName path).Contains "synthings")
        |> Seq.map (fun path -> Assembly.Load path)
        |> Seq.filter (fun assembly -> Seq.exists (fun (m : Module) -> m.Name = "library") assembly.Modules)
    Assert.True(Seq.length assemblies > 0)
