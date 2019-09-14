namespace synthings.core

type Graph =
    {
        Root : Machine
        Machines : Map<System.Guid, Machine>;
        Connections : Map<System.Guid, System.Guid>
    }

module graph =
    open machine
    
    let empty = {Root = createRelay "Root"; Machines = Map.empty; Connections = Map.empty}
    
    let addMachine (machine : Machine) (graph : Graph) =
        {graph with Machines = Map.add machine.Id machine graph.Machines}
    