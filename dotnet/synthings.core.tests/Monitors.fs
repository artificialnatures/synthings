module synthings.core.tests.Monitors

open Xunit
open synthings.core

[<Fact>]
let ``Monitor the output of a machine`` () =
    let monitor = monitor.create()
    let relayGraph =
        graph.empty
        |> graph.addMachine monitor.Machine
        |> graph.connectToRoot monitor.Machine.Id
    let expected = signal.createSignalSequence 0.0 1.0 [0.0; 1.0; 2.0; 3.0; 4.0; 5.0]
    Seq.iter (fun signal -> graph.induce relayGraph signal) expected
    let result =
        Seq.zip expected monitor.Recording
        |> Seq.forall (fun (expected, actual) -> expected.Value = actual.Value)
    Assert.True(result)
