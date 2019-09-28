module synthings.scalar.tests.Envelopes

open Xunit
open synthings.core
open synthings.scalar

[<Fact>]
let ``Linear decay produces descending values`` () =
    let scalarLibrary = ScalarLibrary()
    let linearDecayMachine = scalarLibrary.createMachine EnvelopeBehavior.LinearDecay
    let mutable actual = List.empty
    let record (signal : Signal) =
        actual <- List.append actual [signal.Value]
        signal
    let monitoredMachine = machine.createMonitor linearDecayMachine record
    let messages =
        Signal.createUniformSeries (time.now ()) 0.0 10.0 1.0 1.0
        |> Seq.map message.packWithoutForwarding
    Seq.iter monitoredMachine.Input messages
    Assert.True(number.isDescending actual)
