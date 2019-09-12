module synthings.core.tests.DecayTests

open Xunit
open synthings.core

[<Theory>]
[<InlineData(0.0, 1.0, 0.0, 1.0)>]
[<InlineData(0.0, 1.0, 0.5, 0.5)>]
[<InlineData(0.0, 1.0, 1.0, 0.0)>]
[<InlineData(0.0, 1.0, 1.5, 0.0)>]
[<InlineData(3.0, 5.0, 3.0, 1.0)>]
[<InlineData(3.0, 5.0, 5.5, 0.5)>]
[<InlineData(3.0, 5.0, 8.0, 0.0)>]
[<InlineData(3.0, 5.0, 10.0, 0.0)>]
let ``Linear decay`` startTime duration time expected =
    let actual = envelope.linear startTime duration time
    Assert.Equal(expected, actual, number.DecimalPrecision)

[<Fact>]
let ``Linear decay produces descending values`` () =
    let decay = envelope.createWorker {Name = "Decay"; Envelope = envelope.Envelope.Linear; Start = 0.0; Duration = 10.0}
    let result =
        signal.createSignalSequence 0.0 10.0 1.0 1.0
        |> Seq.map decay
        |> Seq.map signal.unpack
        |> Seq.pairwise
        |> Seq.forall (fun pair -> fst pair > snd pair)
    Assert.True(result)
