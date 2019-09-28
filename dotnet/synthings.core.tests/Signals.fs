module synthings.core.tests.Signals

open Xunit
open synthings.core

[<Fact>]
let ``A signal sequence has equally distributed sample times`` () =
    let interval = 1.0
    let result =
        signal.createUniformSignalSequence 0.0 10.0 interval 0.0
        |> Seq.pairwise
        |> Seq.map (fun (first, second) -> signal.timeSpan first second)
        |> Seq.forall (fun span -> number.equalWithinTolerance span interval)
    Assert.True(result)

[<Fact>]
let ``A signal sequence has ascending sample times`` () =
    let result =
        signal.createUniformSignalSequence 0.0 10.0 1.0 0.0
        |> Seq.map signal.secondsSinceEpoch
        |> Seq.pairwise
        |> Seq.forall (fun pair -> fst pair < snd pair)
    Assert.True(result)
