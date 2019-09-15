namespace synthings.core

type Graph =
    {
        Root : Machine
        Machines : Map<System.Guid, Machine>;
        Connections : Connection list
    }

module graph =
    open System
    open machine
    
    let empty =
        let root = createRelay "Root"
        let machines = Map.empty |> Map.add root.Id root
        {Root = root; Machines = machines; Connections = List.empty}
    
    let addMachine (machine : Machine) (graph : Graph) =
        {graph with Machines = Map.add machine.Id machine graph.Machines}
    
    let removeMachine (machine : Machine) (graph : Graph) =
        {graph with Machines = Map.remove machine.Id graph.Machines}
    
    let replaceMachine (machine : Machine) (graph : Graph) =
        graph
        |> removeMachine machine
        |> addMachine machine
    
    let replaceRoot (machine : Machine) (graph : Graph) =
        {graph with Root = machine}
    
    let addConnection (connection : Connection) (graph : Graph) =
        {graph with Connections = List.append graph.Connections [connection]}
    
    let connect (upstreamId : Guid) (downstreamId : Guid) (graph : Graph) =
        let connection = machine.connect (graph.Machines.Item upstreamId) (graph.Machines.Item downstreamId)
        graph
        |> replaceMachine connection.Upstream
        |> addConnection connection
    
    let connectToRoot (downstreamId : Guid) (graph : Graph) =
        let connection = machine.connect graph.Root (graph.Machines.Item downstreamId)
        graph
        |> replaceRoot connection.Upstream
        |> replaceMachine connection.Upstream
        |> addConnection connection
    
    let input (graph : Graph) (machineId : Guid) (signal : Signal) =
        (graph.Machines.Item machineId).Input signal
    