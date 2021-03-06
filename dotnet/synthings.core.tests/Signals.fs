module synthings.core.tests.Signals

open Xunit
open synthings.core

[<Fact>]
let ``A signal sequence has equally distributed sample times`` () =
    let configuration = {Epoch = Instant.now(); StartAtSeconds = 0.0; EndAtSeconds = 10.0; IntervalSeconds = 1.0}
    let result =
        Signal.createUniformSeries configuration EmptySignalValue
        |> List.pairwise
        |> List.map (fun (first, second) -> Signal.timeSpan first second)
        |> List.forall (fun span -> number.equalWithinTolerance span configuration.IntervalSeconds)
    Assert.True(result)

[<Fact>]
let ``A signal sequence has ascending sample times`` () =
    let configuration = {Epoch = Instant.now(); StartAtSeconds = 0.0; EndAtSeconds = 10.0; IntervalSeconds = 1.0}
    let result =
        Signal.createUniformSeries configuration EmptySignalValue
        |> List.map Signal.secondsSinceEpoch
        |> List.pairwise
        |> List.forall (fun pair -> fst pair < snd pair)
    Assert.True(result)

[<Fact>]
let ``Values can be specified in a signal sequence`` () =
    let epoch = Instant.now ()
    let values =
        [0.0; 1.0; 2.0; 3.0; 4.0; 5.0; 6.0; 7.0; 8.0; 9.0; 10.0]
        |> List.map DecimalDouble
    let signals = Signal.createSeries epoch 0.0 1.0 values
    let result =
        signals
        |> List.map Signal.unpack
        |> List.zip values
        |> List.forall (fun (expected, actual) -> expected = actual)
    Assert.True(result)
