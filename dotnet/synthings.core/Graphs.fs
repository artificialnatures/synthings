namespace synthings.core

module Graphs =
    open System
    open Machines
        
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
    