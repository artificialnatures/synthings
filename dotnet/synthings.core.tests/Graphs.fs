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
        graph.empty
        |> graph.addMachine machine1
        |> graph.addMachine receiver
        |> graph.connect machine1.Id receiver.Id //Why is order important?
        |> graph.connectToRoot machine1.Id       //Doesn't work if these lines are swapped...need to update upstream connections
    let signal = signal.createSignal time.now 0.0 0.0
    graph1.Root.Input signal
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
    let graph1 =
        graph.empty
        |> graph.addMachine machine1
        |> graph.addMachine machine2
        |> graph.addMachine machine3
        |> graph.addMachine receiver
        |> graph.connect machine1.Id receiver.Id
        |> graph.connect machine2.Id receiver.Id
        |> graph.connect machine3.Id receiver.Id
        |> graph.connectToRoot machine1.Id
        |> graph.connectToRoot machine2.Id
        |> graph.connectToRoot machine3.Id
    let signal = signal.createSignal time.now 0.0 0.0
    graph1.Root.Input signal
    let expected = [1.0; 2.0; 3.0]
    let result = number.equalsAll expected actual
    Assert.True(result)
