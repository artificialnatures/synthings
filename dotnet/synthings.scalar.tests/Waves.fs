module synthings.scalar.tests.Waves

open Xunit
open synthings.core
open synthings.scalar

[<Fact>]
let ``Sine wave produces oscillating values`` () =
    let scalarLibrary = ScalarLibrary()
    let sineWaveMachine = scalarLibrary.createMachine WaveBehavior.SineWave
    let expected = [0.0; 1.0; 0.0; -1.0; 0.0; 1.0; 0.0; -1.0; 0.0]
    let mutable actual = List.empty
    let record (signal : Signal) =
        actual <- List.append actual [signal.Value]
        signal
    let monitoredMachine = machine.createMonitor sineWaveMachine record
    let messages =
        signal.createSignalSequence 0.0 2.0 0.25 0.0
        |> Seq.map message.packWithoutForwarding
    Seq.iter monitoredMachine.Input messages
    Assert.True(number.listsAreIdentical expected actual)
