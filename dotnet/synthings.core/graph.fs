namespace synthings.core

type Connection =
    {
        Id : Identifier;
        UpstreamId : Identifier;
        DownstreamId : Identifier
    }

type Graph =
    {
        Root : Machine
        Machines : Map<Identifier, Machine>;
        Connections : Map<Identifier, Identifier list>
    }

type Graph with
    static member empty =
        let root = Machine.createRelay()
        let machines = Map.empty |> Map.add root.Id root
        {Root = root; Machines = machines; Connections = Map.empty}
    
    static member addMachine (machine : Machine) (graph : Graph) =
        {graph with Machines = Map.add machine.Id machine graph.Machines}
    
    static member removeMachine (machine : Machine) (graph : Graph) =
        {graph with Machines = Map.remove machine.Id graph.Machines}
    
    static member replaceMachine (machine : Machine) (graph : Graph) =
        graph
        |> Graph.removeMachine machine
        |> Graph.addMachine machine
    
    static member replaceRoot (machine : Machine) (graph : Graph) =
        {graph with Root = machine}
    
    static member addDownstreamConnection (upstreamId : Identifier) (downstreamId : Identifier) (connectionMap : Map<Identifier, Identifier list>) =
        let connections = List.append (connectionMap.Item upstreamId) [downstreamId]
        connectionMap
        |> Map.remove upstreamId
        |> Map.add upstreamId connections
    
    static member connect (upstreamId : Identifier) (downstreamId : Identifier) (graph : Graph) =
        let upstreamHasConnections = graph.Connections.ContainsKey upstreamId
        let connectionMap =
            match upstreamHasConnections with
            | true -> Graph.addDownstreamConnection upstreamId downstreamId graph.Connections
            | false -> Map.add upstreamId [downstreamId] graph.Connections
        {graph with Connections = connectionMap}
    
    static member connectToRoot (downstreamId : Identifier) (graph : Graph) =
        Graph.connect graph.Root.Id downstreamId graph
    
    static member connectMonitor (machineId : Identifier) (window : Window) (graph : Graph) =
        let monitor = Monitor(window)
        let revisedGraph = Graph.connect machineId monitor.Machine.Id graph
        (revisedGraph, monitor)
    
    static member sendTo (graph : Graph) (machineId : Identifier) (message : Message) =
        let downstream = graph.Machines.Item machineId
        downstream.Input message
    
    static member sendFrom (graph : Graph) (upstreamId : Identifier) (message : Message) =
        let upstreamHasConnections = graph.Connections.ContainsKey upstreamId
        if upstreamHasConnections then
            let downstreamConnections = graph.Connections.Item upstreamId
            List.iter (fun downstreamId -> Graph.sendTo graph downstreamId message) downstreamConnections
    
    static member induce (graph : Graph) (signal : Signal) =
        graph.Root.Input {Signal = signal; Forwarder = Graph.sendFrom graph}
    