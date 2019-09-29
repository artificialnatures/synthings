namespace synthings.core

type Machine =
    {
        Id : System.Guid;
        Name : string;
        Input : Message -> unit;
        Behavior : Behavior;
        DownstreamConnections : System.Guid list
    }

type Machine with
    static member input (id : System.Guid) (behavior : Behavior) (message : Message) =
        message.Forwarder id {message with Signal = behavior message.Signal}
    
    static member createMachine name behavior =
        let id = System.Guid.NewGuid()
        {Id = id; Name = name; Behavior = behavior; Input = Machine.input id behavior; DownstreamConnections = List.empty}
    
    static member createRelay () =
        let relay (signal : Signal) = signal
        Machine.createMachine "Relay" relay
    
    static member createError () =
        let error (signal : Signal) = {signal with Value = 0.0}
        Machine.createMachine "Error" error
    
    static member createMonitor (originalMachine : Machine) (record : Behavior) =
        let behavior = originalMachine.Behavior >> record
        {originalMachine with Behavior = behavior; Input = Machine.input originalMachine.Id behavior}
