namespace synthings.core

type Machine =
    {
        Id : System.Guid;
        Name : string;
        Input : Port;
        Behavior : Behavior;
        DownstreamConnections : System.Guid list
    }

module machine =
    open System
    open behavior
    
    let input (id : Guid) (behavior : Behavior) (message : Message) =
        message.Forwarder id {message with Signal = behavior message.Signal}
    
    let createMachine name behavior =
        let id = Guid.NewGuid()
        {Id = id; Name = name; Behavior = behavior; Input = input id behavior; DownstreamConnections = List.empty}
    
    let createRelay name =
        let id = Guid.NewGuid()
        let behavior = relay
        {Id = id; Name = name; Behavior = behavior; Input = input id behavior; DownstreamConnections = List.empty}
    
    let createMonitor (machine : Machine) =
        let mutable state = List.empty
        let recordState (signal : Signal) =
            state <- List.append state [signal.Value]
            signal
        let compositeBehavior = machine.Behavior >> recordState
        let compositeMachine = createMachine machine.Name compositeBehavior
        compositeMachine, state
    