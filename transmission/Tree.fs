namespace synthings.transmission

type RecursionLevel =
    | FirstPass
    | Default

type TreeEntity<'entity> =
    {
        entity : 'entity
        id : Identifier
        parentId : Identifier option
    }

type Tree<'entity> =
    | Node of 'entity * Tree<'entity> list
    | Leaf of 'entity

module Tree =
    let identify parentId entityId tree =
        let rec identify parentId entityId tree =
            let entityId =
                match entityId with
                | Some entityId -> entityId
                | None -> Identifier.create ()
            match tree with
            | Node (entity, children) ->
                let treeEntity =
                    {
                        entity = entity
                        id = entityId
                        parentId = parentId
                    }
                let children = List.map (fun child -> identify (Some entityId) None child) children
                //TODO: assign order to each sibling
                Node (treeEntity, children)
            | Leaf entity ->
                {
                    entity = entity
                    id = entityId
                    parentId = parentId
                }
                |> Leaf
        identify parentId entityId tree
    
    let collect collector tree =
        let rec collect recursionLevel collection tree =
            let collectedEntity = 
                match recursionLevel with
                | FirstPass -> collector FirstPass tree
                | Default -> collector Default tree
            match tree with
            | Node (_, children) ->
                List.map (fun child -> collect Default List.empty child) children
                |> List.append [
                        collection
                        [collectedEntity]
                    ]
                |> List.concat
            | Leaf _ ->
                [
                    collection
                    [collectedEntity]
                ]
                |> List.concat
        collect FirstPass List.empty tree
