module synthings.scalar.tests.Waves

open Xunit
open synthings.core
open synthings.scalar
open synthings.scalar.library

[<Fact>]
let ``Sine wave produces oscillating values`` () =
    let parameters = WaveParameters(1.0, 2.0)
    let wave = library.createBehavior Scalar.SineWave parameters
    let isValid = number.equalsAny [1.0; -1.0]
    let result =
        signal.createSignalSequence 0.25 10.25 0.5 0.0
        |> Seq.map wave
        |> Seq.map signal.unpack
        |> Seq.forall isValid
    Assert.True(result)
