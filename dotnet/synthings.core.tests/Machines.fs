module synthings.core.tests.Machines

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
    let changes = machine.connect incrementer receiver
    let testSignal = signal.createSignal time.now 0.0 0.0
    changes.Upstream.Input testSignal
    Assert.True(number.equalWithinTolerance result 1.0)

[<Fact>]
let ``Connect two machines to one`` () =
    let increment signal = {signal with Value = signal.Value + 1.0}
    let incrementer = machine.createMachine "Incrementer" increment
    let add2 signal = {signal with Value = signal.Value + 2.0}
    let adder = machine.createMachine "Add 2" add2
    let mutable result = 0.0
    let updateResult signal =
         result <- result + signal.Value
         signal
    let receiver = machine.createMachine "Receiver" updateResult
    let changes1 = machine.connect incrementer receiver
    let signal1 = signal.createSignal time.now 0.0 0.0
    changes1.Upstream.Input signal1
    let changes2 = machine.connect adder receiver
    let signal2 = signal.createSignal time.now 0.0 0.0
    changes2.Upstream.Input signal2
    Assert.True(number.equalWithinTolerance result 3.0)
