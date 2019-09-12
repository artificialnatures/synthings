module synthings.core.tests.FiguringOutHowToBuildGraphs

open Xunit
open synthings.core

[<Fact>]
let ``Build wiggle graph`` () =
    let waveConfiguration : wave.Configuration = {Name = "Sine"; Wave = wave.Wave.Sine; Period = 1.0; Amplitude = 10.0}
    let waveBehavior = wave.createWorker waveConfiguration
    let decayConfiguration : envelope.Configuration = {Name = "Decay"; Envelope = envelope.Envelope.Linear; Start = 0.0; Duration = 10.0}
    let decayBehavior = envelope.createWorker decayConfiguration
    let compositeBehavior = waveBehavior >> decayBehavior
    let result =
        signal.createSignalSequence 0.25 10.25 1.0 0.0
        |> Seq.map compositeBehavior
        |> Seq.map signal.unpack
        |> Seq.pairwise
        |> Seq.forall (fun pair -> fst pair > snd pair)
    Assert.True(result)
