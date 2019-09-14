module synthings.scalar.tests.EnvelopeTests

open Xunit
open synthings.core
open synthings.scalar
open synthings.scalar.library

[<Fact>]
let ``Linear decay produces descending values`` () =
    let parameters = EnvelopeParameters(0.0, 10.0)
    let decay = library.createBehavior Scalar.LinearDecay parameters
    let result =
        signal.createSignalSequence 0.0 10.0 1.0 1.0
        |> Seq.map decay
        |> Seq.map signal.unpack
        |> Seq.pairwise
        |> Seq.forall (fun pair -> fst pair > snd pair)
    Assert.True(result)
