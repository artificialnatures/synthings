namespace synthings.transmission

type ChangeSet<'entity> = Operation<'entity> list

module ChangeSet =
    let undo changeSet =
        List.map Operation.invert changeSet |> List.rev
    
    let validate entityTable proposal =
        match proposal with
        | Initialize _ ->
            Ok proposal
        | Add addProposal ->
            match EntityTable.resolveIdentifier entityTable addProposal.parent with
            | Ok _ -> Ok proposal
            | Error message -> Error message
        | Replace replaceProposal ->
            match EntityTable.resolveIdentifier entityTable replaceProposal.entityToReplace with
            | Ok _ -> Ok proposal
            | Error message -> Error message
        | Remove removeProposal ->
            match EntityTable.resolveIdentifier entityTable removeProposal.entityToRemove with
            | Ok _ -> Ok proposal
            | Error message -> Error message
    
    let create reorder _ tree =
        let treeEntity =
            match tree with
            | Node (treeEntity, _) -> treeEntity
            | Leaf treeEntity -> treeEntity
        let createOperation =
            Create {
                entityId = treeEntity.id
                entity = treeEntity.entity
            }
        match treeEntity.parentId with
        | Some parentId ->
            let operations =
                [
                    createOperation
                    Parent {
                        entityId = treeEntity.id
                        parentId = parentId 
                    }
                ]
            match reorder with
            | Some reorder ->
                match reorder treeEntity.id with
                | Some reorderOperation ->
                    List.append operations [ reorderOperation ]
                | None -> operations
            | None -> operations
        | None -> [ createOperation ]
    
    let delete _ tree =
            let treeEntity =
                match tree with
                | Node (treeEntity, _) -> treeEntity
                | Leaf treeEntity -> treeEntity
            let deleteOperation =
                Delete {
                    entityId = treeEntity.id
                    entity = treeEntity.entity
                }
            match treeEntity.parentId with
            | Some parentId ->
                [
                    Orphan {
                        entityId = treeEntity.id
                        parentId = parentId 
                    }
                    deleteOperation
                ]
            | None -> [ deleteOperation ]
    
    let initialize entityTable proposal =
        let deleteOperations =
            if EntityTable.isEmpty entityTable
            then List.empty
            else
                let treeToBeDeleted = EntityTable.buildDetailedTree None entityTable
                Tree.collect delete treeToBeDeleted
                |> List.rev
                |> List.concat
        let treeToBeCreated = Tree.identify None None proposal.initialTree
        let create = create None
        let createOperations =
            Tree.collect create treeToBeCreated
            |> List.concat
        List.append deleteOperations createOperations
        |> Ok

    let add entityTable proposal =
        match EntityTable.resolveIdentifier entityTable proposal.parent with
        | Ok parentId ->
            let tree = Tree.identify (Some parentId) None proposal.entityToAdd
            let reorder childId =
                match EntityTable.resolveChildOrder entityTable proposal.parent childId proposal.order with
                | Some (order, priorOrder) ->
                    Reorder {
                        entityId = parentId
                        order = order
                        priorOrder = priorOrder 
                    }
                    |> Some
                | None -> None
            let create = create (Some reorder)
            Tree.collect create tree
            |> List.concat
            |> Ok
        | Error message -> Error message

    let replace entityTable proposal =
        match EntityTable.resolveIdentifierAndParent entityTable proposal.entityToReplace with
        | Ok (entityId, parentId) ->
            let treeToBeDeleted = EntityTable.buildDetailedTree (Some entityId) entityTable
            let deleteOperations =
                Tree.collect delete treeToBeDeleted
                |> List.rev
                |> List.concat
            let create = create None
            let createOperations =
                Tree.identify (Some parentId) (Some entityId) proposal.replacement
                |> Tree.collect create
                |> List.concat
            List.append deleteOperations createOperations
            |> Ok
        | Error message -> failwith message
    
    let remove entityTable proposal =
        match EntityTable.resolveIdentifier entityTable proposal.entityToRemove with
        | Ok entityId ->
            EntityTable.buildDetailedTree (Some entityId) entityTable
            |> Tree.collect delete
            |> List.rev
            |> List.concat
            |> Ok
        | Error message -> Error message
    
    let assemble entityTable proposal =
        match validate entityTable proposal with
        | Ok proposal ->
            match proposal with
            | Initialize proposal -> initialize entityTable proposal
            | Add proposal -> add entityTable proposal
            | Replace proposal -> replace entityTable proposal
            | Remove proposal -> remove entityTable proposal
        | Error message -> Error message
