module synthings.core.tests.MovementsTests

open Xunit
open synthings.core

[<Fact>]
let Oscillate () =
    let slowOscillate = movement.oscillate 1.0 1.0
    let sampleTimes = seq {0.0 .. 0.25 .. 10.5}
    let isValid = number.equalsAny [0.0; 0.5; -0.5]
    let result =
        sampleTimes
        |> Seq.map (fun step -> slowOscillate step)
        |> Seq.filter isValid
        |> Seq.length = Seq.length sampleTimes
    Assert.True(result)

[<Fact>]
let Wiggle () =
    let slowWiggle = movement.wiggle 0.0 1.0 1.0 10.0
    let sampleTimes = seq {0.25 .. 1.0 .. 10.25}
    let result =
        sampleTimes
        |> Seq.map (fun step -> slowWiggle step)
        |> Seq.pairwise
        |> Seq.forall (fun pair -> fst pair > snd pair)
    Assert.True(result)

