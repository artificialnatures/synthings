namespace synthings.core

type Connection =
    {
        Id : System.Guid;
        UpstreamId : System.Guid;
        DownstreamId : System.Guid
    }

type Graph =
    {
        Root : Machine
        Machines : Map<System.Guid, Machine>;
        Connections : Map<System.Guid, System.Guid list>
    }

module graph =
    open System
    open machine
    
    let empty =
        let root = createRelay "Root"
        let machines = Map.empty |> Map.add root.Id root
        {Root = root; Machines = machines; Connections = Map.empty}
    
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
    
    let addDownstreamConnection (upstreamId : Guid) (downstreamId : Guid) (connectionMap : Map<Guid, Guid list>) =
        let connections = List.append (connectionMap.Item upstreamId) [downstreamId]
        connectionMap
        |> Map.remove upstreamId
        |> Map.add upstreamId connections
    
    let connect (upstreamId : Guid) (downstreamId : Guid) (graph : Graph) =
        let upstreamHasConnections = graph.Connections.ContainsKey upstreamId
        let connectionMap =
            match upstreamHasConnections with
            | true -> addDownstreamConnection upstreamId downstreamId graph.Connections
            | false -> Map.add upstreamId [downstreamId] graph.Connections
        {graph with Connections = connectionMap}
    
    let connectToRoot (downstreamId : Guid) (graph : Graph) =
        connect graph.Root.Id downstreamId graph
    
    let sendTo (graph : Graph) (machineId : Guid) (message : Message) =
        let downstream = graph.Machines.Item machineId
        downstream.Input message
    
    let sendFrom (graph : Graph) (upstreamId : Guid) (message : Message) =
        let upstreamHasConnections = graph.Connections.ContainsKey upstreamId
        if upstreamHasConnections then
            let downstreamConnections = graph.Connections.Item upstreamId
            List.iter (fun downstreamId -> sendTo graph downstreamId message) downstreamConnections
    
    let induce (graph : Graph) (signal : Signal) =
        graph.Root.Input {Signal = signal; Forwarder = sendFrom graph}
    