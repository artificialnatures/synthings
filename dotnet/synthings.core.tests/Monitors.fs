module synthings.core.tests.Monitors

open Xunit
open synthings.core

[<Fact>]
let ``Monitor the output of a machine`` () =
    let monitor = Monitor(FrameLimit(10))
    let relayGraph =
        graph.empty
        |> graph.addMachine monitor.Machine
        |> graph.connectToRoot monitor.Machine.Id
    let epoch = time.now ()
    let values = [0.0; 1.0; 2.0; 3.0; 4.0; 5.0]
    let expected = Signal.createSeries epoch 0.0 1.0 values
    Seq.iter (fun signal -> graph.induce relayGraph signal) expected
    let result =
        if monitor.Recording.Signals.Length <> Seq.length expected then false else
            List.zip expected monitor.Recording.Signals
            |> List.forall (fun (expected, actual) -> expected.Value = actual.Value)
    Assert.True(result)

[<Fact>]
let ``Limiting the recording capacity of a monitor with FrameLimit`` () =
    let monitor = Monitor(FrameLimit(5))
    let relayGraph =
        graph.empty
        |> graph.addMachine monitor.Machine
        |> graph.connectToRoot monitor.Machine.Id
    let epoch = time.now ()
    let values = [0.0; 1.0; 2.0; 3.0; 4.0; 5.0; 6.0; 7.0; 8.0; 9.0; 10.0]
    let inputs = Signal.createSeries epoch 0.0 1.0 values
    List.iter (fun signal -> graph.induce relayGraph signal) inputs
    let expected = Signal.createSeries epoch 6.0 1.0 values.[6..]
    let result =
        if monitor.Recording.Signals.Length <> Seq.length expected then false else
            List.zip expected monitor.Recording.Signals
            |> List.forall (fun (expected, actual) -> expected.Value = actual.Value)
    Assert.True(result)

[<Fact>]
let ``Limiting the recording capacity of a monitor with TimeLimit`` () =
    let monitor = Monitor(TimeLimit(5.0))
    let relayGraph =
        graph.empty
        |> graph.addMachine monitor.Machine
        |> graph.connectToRoot monitor.Machine.Id
    let epoch = time.now ()
    let values = [0.0; 1.0; 2.0; 3.0; 4.0; 5.0; 6.0; 7.0; 8.0; 9.0; 10.0]
    let inputs = Signal.createSeries epoch 0.0 1.0 values
    let expected = Signal.createSeries epoch 5.0 1.0 values.[5..]
    Seq.iter (fun signal -> graph.induce relayGraph signal) inputs
    let result =
        if monitor.Recording.Signals.Length <> Seq.length expected then false else
            List.zip expected monitor.Recording.Signals
            |> List.forall (fun (expected, actual) -> expected.Value = actual.Value)
    Assert.True(result)

[<Fact>]
let ``Truncate by time`` () =
    let epoch = time.now ()
    let values = [0.0; 1.0; 2.0; 3.0; 4.0; 5.0; 6.0; 7.0; 8.0; 9.0; 10.0]
    let inputs = Signal.createSeries epoch 0.0 1.0 values
    let actualSignals = window.truncateByTime 5.0 inputs
    let expectedSignals = window.truncateByCount 6 inputs
    let result =
        List.zip expectedSignals actualSignals
        |> List.forall (fun (expected, actual) -> expected.Value = actual.Value)
    Assert.True(result)
    
