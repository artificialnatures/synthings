namespace synthings.core

module graph =
    open System
    open machine
        
    type Connection =
        {
            Id : Guid;
            Upstream : Guid;
            Downstream : Guid
        }
    
    type Graph =
        {
            Machines : Machine list;
            Connections : Connection list
        }
    
    let emptyGraph = { Machines = List.empty; Connections = List.empty }
    