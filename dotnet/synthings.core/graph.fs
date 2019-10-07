namespace synthings.core

type Graph =
    {
        Root : Machine
        Machines : Map<Identifier, Machine>;
        Connections : ConnectionSet
    }

type Graph with
    static member empty =
        let root = {Machine.createRelay() with Name = "Root"}
        let machines = Map.empty |> Map.add root.Id root
        {Root = root; Machines = machines; Connections = ConnectionSet.empty}
    
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
        {graph with Connections = ConnectionSet.connect upstreamId downstreamId graph.Connections}
    
    static member connectToRoot (downstreamId : Identifier) (graph : Graph) =
        {graph with Connections = ConnectionSet.connect graph.Root.Id downstreamId graph.Connections}
    
    static member sendTo (graph : Graph) (machineId : Identifier) (message : Message) =
        let downstream = graph.Machines.Item machineId
        downstream.Input message
    
    static member sendFrom (graph : Graph) (upstreamId : Identifier) (message : Message) =
        ConnectionSet.downstreamConnections upstreamId graph.Connections
        |> List.iter (fun connection -> Graph.sendTo graph connection.DownstreamId message)
    
    static member induceInternal (graph : Graph) (signal : Signal) =
        graph.Root.Input {Signal = signal; Forwarder = Graph.sendFrom graph}
    static member induce (forwarder : Forwarder) (graph : Graph) (signal : Signal) =
        graph.Root.Input {Signal = signal; Forwarder = forwarder}
