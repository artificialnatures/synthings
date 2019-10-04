namespace synthings.core

type Connection =
    {
        Id : Identifier;
        UpstreamId : Identifier;
        DownstreamId : Identifier
    }

type Connection with
    static member internal create (upstreamId : Identifier) (downstreamId : Identifier) =
        {Id = Identifier.create(); UpstreamId = upstreamId; DownstreamId = downstreamId}

type ConnectionSet =
    {
        List : Connection list;
        DownstreamMap : Map<Identifier, Connection list>;
        UpstreamMap : Map<Identifier, Connection list>
    }

type ConnectionSet with
    static member empty = {List = List.empty; DownstreamMap = Map.empty; UpstreamMap = Map.empty}
    static member hasDownstreamConnections (upstreamId : Identifier) (connectionSet : ConnectionSet) =
        Map.containsKey upstreamId connectionSet.DownstreamMap
    static member hasUpstreamConnections (upstreamId : Identifier) (connectionSet : ConnectionSet) =
        Map.containsKey upstreamId connectionSet.UpstreamMap
    static member downstreamConnections (upstreamId : Identifier) (connectionSet : ConnectionSet) =
        if not (ConnectionSet.hasDownstreamConnections upstreamId connectionSet) then List.empty else
            connectionSet.DownstreamMap.Item upstreamId
    static member upstreamConnections (downstreamId : Identifier) (connectionSet : ConnectionSet) =
        if not (ConnectionSet.hasUpstreamConnections downstreamId connectionSet) then List.empty else
            connectionSet.UpstreamMap.Item downstreamId
    static member addToDownstreamMap (connection : Connection) (connectionSet : ConnectionSet) =
        let connections = List.append (ConnectionSet.downstreamConnections connection.UpstreamId connectionSet) [connection]
        Map.add connection.UpstreamId connections connectionSet.DownstreamMap
    static member addToUpstreamMap (connection : Connection) (connectionSet : ConnectionSet) =
        let connections = List.append (ConnectionSet.upstreamConnections connection.DownstreamId connectionSet) [connection]
        Map.add connection.DownstreamId connections connectionSet.UpstreamMap
    static member tryFindConnection (upstreamId : Identifier) (downstreamId : Identifier) (connectionSet : ConnectionSet) =
        ConnectionSet.downstreamConnections upstreamId connectionSet
        |> List.tryFind (fun connection -> connection.DownstreamId = downstreamId) 
    static member findConnection (upstreamId : Identifier) (downstreamId : Identifier) (connectionSet : ConnectionSet) =
        let connection = ConnectionSet.tryFindConnection upstreamId downstreamId connectionSet
        match connection with
        | Some connection -> connection
        | None -> failwith "Connection not found."
    static member connectionExists (upstreamId : Identifier) (downstreamId : Identifier) (connectionSet : ConnectionSet) =
        let connection = ConnectionSet.tryFindConnection upstreamId downstreamId connectionSet
        match connection with
        | Some connection -> true
        | None -> false
    static member connect (upstreamId : Identifier) (downstreamId : Identifier) (connectionSet : ConnectionSet) =
        if ConnectionSet.connectionExists upstreamId downstreamId connectionSet then connectionSet else
            let connection = Connection.create upstreamId downstreamId
            let list = List.append connectionSet.List [connection]
            let downstreamMap = ConnectionSet.addToDownstreamMap connection connectionSet
            let upstreamMap = ConnectionSet.addToUpstreamMap connection connectionSet
            {List = list; DownstreamMap = downstreamMap; UpstreamMap = upstreamMap}
