module synthings.core.tests.CreatingSignals

open Xunit
open synthings.core

[<Fact>]
let ``Creating a sequence of signals`` () =
    let sampleTimes = seq {0.0 .. 0.25 .. 10.5}
    let sampleSignals =
        sampleTimes
        |> Signals.createTimeSignals
    let result =
        sampleSignals
        |> Seq.map (fun signal -> Signals.secondsSinceEpoch signal)
        |> Seq.forall (fun deltaTime -> Seq.contains deltaTime sampleTimes)
    Assert.True(result)
