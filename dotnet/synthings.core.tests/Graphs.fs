module synthings.core.tests.Graphs

open Xunit
open synthings.core

[<Fact>]
let ``A graph with 2 connected machines`` () =
    let add amount signal =
        {signal with Value = signal.Value + amount}
    let mutable actual = 0.0
    let updateResult signal =
        actual <- signal.Value
        signal
    let machine1 = machine.createMachine "Machine 1" (add 1.0)
    let receiver = machine.createMachine "Receiver" updateResult
    let graph1 =
        Graph.empty
        |> Graph.addMachine machine1
        |> Graph.addMachine receiver
        |> Graph.connectToRoot machine1.Id
        |> Graph.connect machine1.Id receiver.Id
    let epoch = time.now ()
    let signal = Signal.createSample epoch 0.0 0.0
    Graph.induce graph1 signal
    Assert.True(number.equalWithinTolerance 1.0 actual)

[<Fact>]
let ``Build a calculation graph`` () =
    let add amount signal = {signal with Value = signal.Value + amount}
    let mutable actual : float list = List.empty
    let updateResult signal =
        actual <- List.append [signal.Value] actual
        signal
    let machine1 = machine.createMachine "Machine 1" (add 1.0)
    let machine2 = machine.createMachine "Machine 2" (add 1.0)
    let machine3 = machine.createMachine "Machine 3" (add 1.0)
    let receiver = machine.createMachine "Receiver" updateResult
    let calculationGraph =
        Graph.empty
        |> Graph.addMachine machine1
        |> Graph.addMachine machine2
        |> Graph.addMachine machine3
        |> Graph.addMachine receiver
        |> Graph.connect machine1.Id receiver.Id
        |> Graph.connect machine2.Id receiver.Id
        |> Graph.connect machine3.Id receiver.Id
        |> Graph.connectToRoot machine1.Id
        |> Graph.connectToRoot machine2.Id
        |> Graph.connectToRoot machine3.Id
    let epoch = time.now ()
    let signal = Signal.createSample epoch 0.0 0.0
    Graph.induce calculationGraph signal
    let expected = [1.0; 2.0; 3.0]
    let result = number.actualEqualsAnyInExpected expected actual
    Assert.True(result)
