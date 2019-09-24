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
    
    let input (id : Guid) (behavior : Behavior) (message : Message) =
        message.Forwarder id {message with Signal = behavior message.Signal}
    
    let createMachine name behavior =
        let id = Guid.NewGuid()
        {Id = id; Name = name; Behavior = behavior; Input = input id behavior; DownstreamConnections = List.empty}
    
    let createRelay () =
        let relay (signal : Signal) = signal
        createMachine "Relay" relay
    
    let createError () =
        let error (signal : Signal) = {signal with Value = 0.0}
        createMachine "Error" error
