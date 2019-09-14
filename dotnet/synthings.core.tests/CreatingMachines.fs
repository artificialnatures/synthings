module synthings.core.tests.CreatingMachines

open Xunit
open synthings.core

[<Fact>]
let ``Connect two simple machines`` () =
    let increment signal = {signal with Value = signal.Value + 1.0}
    let incrementer = machine.createMachine "Incrementer" increment
    let mutable result = 0.0
    let updateResult signal =
        result <- signal.Value
        signal
    let receiver = machine.createMachine "Receiver" updateResult
    let connection = machine.connect incrementer receiver
    let testSignal = signal.createSignal time.now 0.0 0.0
    connection.Upstream.Input testSignal
    Assert.True(number.equalWithinTolerance result 1.0)
