namespace synthings.core

type Port = Signal -> unit

type Machine =
    {
        Id : System.Guid;
        Name : string;
        Behavior : Behavior;
        Input : Port
        Outputs : Port list
    }

type Connection =
    {
        Id : System.Guid;
        Upstream : Machine;
        Downstream : Machine
    }

module machine =
    open System
    open behavior
    
    let emptyInput (signal : Signal) = ()
    
    let emptyOutput : Port list = List.empty
    
    let forwardToAll (outputs : Port list) (signal : Signal) =
        List.iter (fun output -> output signal) outputs
    
    let forward (behavior : Behavior) (outputs : Port list) (signal : Signal) =
        behavior signal
        |> forwardToAll outputs
    
    let createMachine name behavior =
        let input = forward behavior emptyOutput
        {Id = Guid.NewGuid(); Name = name; Behavior = behavior; Input = input; Outputs =  emptyOutput}
    
    let createRelay name =
        let id = Guid.NewGuid()
        let behavior = relay
        let outputs = emptyOutput
        let input = forward behavior outputs
        {Id = id; Name = name; Behavior = behavior; Input = input; Outputs = outputs}
    
    let connect (upstream : Machine) (downstream : Machine) =
        let upstreamOutputs = List.append upstream.Outputs [downstream.Input]
        let upstreamInput = forward upstream.Behavior upstreamOutputs
        let connectedUpstream = {upstream with Input = upstreamInput; Outputs = upstreamOutputs}
        {Id = Guid.NewGuid(); Upstream = connectedUpstream; Downstream = downstream}
    