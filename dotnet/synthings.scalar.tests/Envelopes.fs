module synthings.scalar.tests.Envelopes

open Xunit
open synthings.core
open synthings.scalar

[<Fact>]
let ``Linear decay produces descending values`` () =
    let scalarLibrary = ScalarLibrary()
    let behaviorDescriptor = scalarLibrary.behaviorWithIdentifier EnvelopeBehavior.LinearDecay
    let linearDecayMachine = scalarLibrary.createMachine behaviorDescriptor
    let mutable actual = List.empty
    let record (signal : Signal) =
        actual <- List.append actual [signal.Value]
        signal
    let monitoredMachine = Machine.createMonitor linearDecayMachine record
    let messages =
        Signal.createUniformSeries (Instant.now ()) 0.0 10.0 1.0 1.0
        |> Seq.map Message.packWithoutForwarding
    Seq.iter monitoredMachine.Input messages
    Assert.True(number.isDescending actual)
