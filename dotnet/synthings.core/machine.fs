namespace synthings.core

type Machine =
    {
        Id : Identifier;
        Name : string;
        Input : Message -> unit;
        Behavior : Behavior
    }
    interface Subject

type Machine with
    static member input (id : Identifier) (behavior : Behavior) (message : Message) =
        message.Forwarder id {message with Signal = behavior message.Signal}
    
    static member createMachine name behavior =
        let id = Identifier.create ()
        {Id = id; Name = name; Behavior = behavior; Input = Machine.input id behavior}
