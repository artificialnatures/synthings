module synthings.core.tests.ConnectingMachines

open Xunit
open System
open synthings.core
open Machines

[<Fact>]
let ``Connecting three machines`` () =
    let machineA = Relay.createMachine "Relay A"
    let machineB = Relay.createMachine "Relay B"
    let machineC = Relay.createMachine "Relay C"
    let wire1 = Machines.connect machineA machineB
    let wire2 = Machines.connect machineB machineC
    let sampleSignals =
        seq {1.0..10.0}
        |> Signals.createTimeSignals
    let mutable observedSignals = Signals.emptySequence
    let observe signal = observedSignals <- Seq.append observedSignals (Seq.singleton signal)
    let signalObserver = Reactive.createObserver observe
    let wire3 = machineC.Output.Subscribe signalObserver
    Seq.iter (fun signal -> machineA.Input.OnNext signal) sampleSignals
    let signal = Seq.item 0 sampleSignals
    Machines.drive machineA signal
    Assert.Equal(Seq.length sampleSignals, Seq.length observedSignals)
