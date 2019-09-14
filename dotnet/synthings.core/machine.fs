namespace synthings.core

type Machine =
    {
        Id : System.Guid;
        Name : string;
        Behavior : Behavior;
    }

module machine =
    open System
    open behavior
    
    let create name behavior = { Id = Guid.NewGuid(); Name = name; Behavior = behavior }
    
    let createRelay name = { Id = Guid.NewGuid(); Name = name; Behavior = relay }
    