module synthings.core.tests.Graphs

open Xunit
open synthings.core

//No such thing as a standalone graph? Have to construct graphs through Application? or at least with reference to a library?
(*
[<Fact>]
let ``A standalone graph with 2 connected machines`` () =
    //Too much nonsense...
    let add (amount : float) (signal : Signal) =
        match signal with
        | :? CoreSignal as coreSignal ->
            match coreSignal with
            | DecimalSingleSignal decimalSingleSignal ->
                let value = DecimalSingle((unbox decimalSingleSignal.Value) + amount)
                let addSignal = Signal.createFromInput decimalSingleSignal value
                DecimalSingleSignal(addSignal) :> Signal
            | _ -> signal
        | _ -> signal
    let mutable actual = 0.0
    let updateResult (signal : Signal) =
        match signal with
        | :? CoreSignal as coreSignal ->
            match coreSignal with
            | DecimalSingleSignal decimalSingleSignal ->
                match decimalSingleSignal.Value with
                | DecimalSingle number ->
                    actual <- number
        signal
    let machine1 = Machine.createMachine "Machine 1" (add 1.0)
    let receiver = Machine.createMachine "Receiver" updateResult
    let graph1 =
        Graph.empty
        |> Graph.addMachine machine1
        |> Graph.addMachine receiver
        |> Graph.connectToRoot machine1.Id
        |> Graph.connect machine1.Id receiver.Id
    let epoch = Instant.now ()
    let signal = Signal.createSample epoch 0.0 0.0
    Graph.induceInternal graph1 signal
    Assert.True(number.equalWithinTolerance 1.0 actual)

[<Fact>]
let ``Build a standalone calculation graph`` () =
    let add amount signal = {signal with Value = signal.Value + amount}
    let mutable actual : float list = List.empty
    let updateResult signal =
        actual <- List.append [signal.Value] actual
        signal
    let machine1 = Machine.createMachine "Machine 1" (add 1.0)
    let machine2 = Machine.createMachine "Machine 2" (add 1.0)
    let machine3 = Machine.createMachine "Machine 3" (add 1.0)
    let receiver = Machine.createMachine "Receiver" updateResult
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
    let epoch = Instant.now ()
    let signal = Signal.createSample epoch 0.0 0.0
    Graph.induceInternal calculationGraph signal
    let expected = [1.0; 2.0; 3.0]
    let result = number.actualEqualsAnyInExpected expected actual
    Assert.True(result)
*)