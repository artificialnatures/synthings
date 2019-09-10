module synthings.core.tests.CreatingMachines

open Xunit
open System
open synthings.core
open synthings.core.Waves

[<Fact>]
let ``Create a sine wave machine`` () =
    let configuration = { Name = "Sine Wave"; Wave = Wave.Sine; Period = 1.0; Amplitude = 2.0  }
    let machine = Waves.createMachine configuration
    let sampleSignals =
        seq {0.0 .. 0.25 .. 10.5}
        |> Signals.createTimeSignals
    let driver = Machines.drive machine
    let mutable observedSignals = Signals.emptySequence
    let signalReceiver = Reactive.createObserver (fun signal ->
        observedSignals <- Seq.append observedSignals (Seq.singleton signal))
    //let inboundSubscription = driver.Subscribe machine.Input //causes stack overflow...
    let outboundSubscription = machine.Output.Subscribe signalReceiver
    for signal in sampleSignals do driver signal
    let filter = Numbers.equalsAny [0.0; 1.0; -1.0]
    let result =
        observedSignals
        |> Seq.map (fun signal -> signal.Value)
        |> Seq.filter filter
        |> Seq.length = Seq.length sampleSignals
    Assert.True(result)

[<Fact>]
let ``Build wiggle by connecting machines`` () =
    let waveMachine = Waves.createMachine { Name = "Sine Wave"; Wave = Wave.Sine; Period = 1.0; Amplitude = 2.0  }
    let decayMachine = Envelopes.createMachine { Name = "Linear Decay"; Envelope = Envelopes.Envelope.Linear; Start = 0.0; Duration = 10.0  }
    let wire = waveMachine.Output.Subscribe decayMachine.Input
    let sampleSignals =
        seq {0.0 .. 0.25 .. 10.5}
        |> Signals.createTimeSignals
    let mutable observedSignals = Signals.emptySequence
    let receive signal = observedSignals <- Seq.append observedSignals (Seq.singleton signal)
    let signalReceiver = Reactive.createObserver receive
    let outboundSubscription = decayMachine.Output.Subscribe signalReceiver
    let driver = Machines.drive waveMachine
    for signal in sampleSignals do driver signal
    let filter = Numbers.equalsNone [0.0]
    let result =
        observedSignals
        |> Seq.map (fun signal -> signal.Value)
        |> Seq.filter filter
        |> Seq.pairwise
        |> Seq.forall (fun pair -> fst pair > snd pair)
    Assert.True(result)
