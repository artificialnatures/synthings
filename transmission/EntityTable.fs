namespace synthings.transmission

type EntityTable<'entity> = 
    {
        rootId : Identifier
        entities : Map<Identifier, 'entity>
        relations : Map<Identifier, Identifier list>
    }

module EntityTable =
    let empty =
        {
            rootId = Identifier.empty
            entities = Map.empty
            relations = Map.empty
        }
    
    let isEmpty entityTable =
        Map.isEmpty entityTable.entities &&
        Map.isEmpty entityTable.relations
    
    let validate (entityTable : EntityTable<'entity>) (identifier : Identifier) =
        if (Map.containsKey identifier entityTable.entities &&
            Map.containsKey identifier entityTable.relations)
        then Ok identifier
        else Error $"Identifier ({identifier}) missing from EntityTable."
    
    let resolveIdentifier entityTable entityReference relativeTo =
        match entityReference with
        | Root ->
            validate entityTable entityTable.rootId
        | Self ->
            validate entityTable relativeTo
        | Ancestor (generations) ->
            let rec findParent generation entityId =
                match Map.tryFindKey (fun _ children -> List.contains entityId children) entityTable.relations with
                | Some parentId -> 
                    if generation < generations
                    then findParent (generation + 1) parentId
                    else validate entityTable parentId
                | None -> validate entityTable entityId
            if generations > 0
            then findParent 1 relativeTo
            else validate entityTable relativeTo
        | Sibling (steps) ->
            match Map.tryFindKey (fun _ children -> List.contains relativeTo children) entityTable.relations with
            | Some parentId -> 
                let siblingIds = entityTable.relations[parentId]
                let entityIndex = List.findIndex (fun siblingId -> siblingId = relativeTo) siblingIds
                let siblingId =
                    match entityIndex + steps with
                    | index when index < 0 -> siblingIds[0]
                    | index when index > (List.length siblingIds) - 1 -> List.last siblingIds
                    | index -> siblingIds[index]
                validate entityTable siblingId
            | None -> validate entityTable relativeTo
        | Specified entityId ->
            validate entityTable entityId
    
    let resolveIdentifierAndParent entityTable entityReference relativeTo =
        match resolveIdentifier entityTable entityReference relativeTo with
        | Ok entityId ->
            let parentId = Map.tryFindKey (fun _ children -> List.contains entityId children) entityTable.relations
            Ok (entityId, parentId)
        | Error message -> Error message
    
    let resolveChildOrder entityTable parentReference relativeTo childId insertionOrder =
        match resolveIdentifier entityTable parentReference relativeTo with
        | Ok parentId ->
            let priorOrder =
                if List.contains childId entityTable.relations[parentId]
                then entityTable.relations[parentId]
                else List.append entityTable.relations[parentId] [ childId ]
            let revisedOrder =
                match insertionOrder with
                | First -> 
                    List.except [ childId ] priorOrder
                    |> List.append [ childId ]
                | Last ->
                    List.append (List.except [ childId ] priorOrder) [ childId ]
                | Precede siblingId ->
                    let idsWithoutChildId = List.except [ childId ] priorOrder
                    match List.tryFindIndex (fun identifier -> identifier = siblingId) idsWithoutChildId with
                    | Some index ->
                        List.insertAt index childId idsWithoutChildId
                    | None -> priorOrder
            if priorOrder = revisedOrder
            then None
            else Some (revisedOrder, priorOrder)
        | Error _ -> None
    
    let executeOperation entityTable operation =
        match operation with
        | Create operation ->
            if isEmpty entityTable
            then
                {
                    rootId = operation.entityId
                    entities = [(operation.entityId, operation.entity)] |> Map.ofList
                    relations = [(operation.entityId, List.empty)] |> Map.ofList
                }
            else
                {entityTable with
                    entities = Map.add operation.entityId operation.entity entityTable.entities
                    relations = Map.add operation.entityId List.empty entityTable.relations
                }
        | Parent operation ->
            let childIds = List.append entityTable.relations[operation.parentId] [ operation.entityId ]
            {entityTable with
                relations = 
                    Map.remove operation.parentId entityTable.relations
                    |> Map.add operation.parentId childIds
            }
        | Reorder operation ->
            {entityTable with
                relations = 
                    Map.remove operation.entityId entityTable.relations
                    |> Map.add operation.entityId operation.order
            }
        | Update operation ->
            {entityTable with
                entities =
                    Map.remove operation.entityId entityTable.entities
                    |> Map.add operation.entityId operation.entity
            }
        | Orphan operation ->
            let childIds = List.except [ operation.entityId ] entityTable.relations[operation.parentId]
            {entityTable with
                relations = 
                    Map.remove operation.parentId entityTable.relations
                    |> Map.add operation.parentId childIds
            }
        | Delete operation ->
            let table =
                {entityTable with
                    entities = Map.remove operation.entityId entityTable.entities
                    relations = Map.remove operation.entityId entityTable.relations
                }
            if isEmpty table
            then {table with rootId = Identifier.empty}
            else table
    
    let apply entityTable changeSet =
        List.fold executeOperation entityTable changeSet
    
    let buildTree rootId entityTable =
        let rec build entityTable entityId =
            let entity = entityTable.entities[entityId]
            let childrenIds = entityTable.relations[entityId]
            if List.isEmpty childrenIds
            then Leaf entity
            else Node (entity, List.map (fun child -> build entityTable child) childrenIds)
        match rootId with
        | None ->
            if isEmpty entityTable
            then failwith "Cannot build tree from empty EntityTable."
            else build entityTable entityTable.rootId
        | Some rootId -> build entityTable rootId
    
    let buildDetailedTree rootId entityTable =
        let rec build entityTable entityId =
            let entity =
                let parent =
                    match Map.tryFindKey (fun _ children -> List.contains entityId children) entityTable.relations with
                    | None -> None
                    | Some parentId -> Some parentId
                {
                    entity = entityTable.entities[entityId]
                    id = entityId
                    parentId = parent
                }
            let childrenIds = entityTable.relations[entityId]
            if List.isEmpty childrenIds
            then Leaf entity
            else Node (entity, List.map (fun child -> build entityTable child) childrenIds)
        match rootId with
        | None ->
            if isEmpty entityTable
            then failwith "Cannot build tree from empty EntityTable."
            else build entityTable entityTable.rootId
        | Some rootId -> build entityTable rootId
        
    
    let fromTree tree =
        let tree =
            Tree.identify None None tree
        let rootId =
            match tree with
            | Node (treeEntity, _) -> treeEntity.id
            | Leaf treeEntity -> treeEntity.id
        let entityTable =
            let collectIdentifierAndEntity _ tree =
                match tree with
                | Node (treeEntity, _) ->
                    (treeEntity.id, treeEntity.entity)
                | Leaf treeEntity ->
                    (treeEntity.id, treeEntity.entity)
            Tree.collect collectIdentifierAndEntity tree
            |> Map.ofList
        let relationsTable =
            let collectRelations _ tree =
                match tree with
                | Node (treeEntity, children) ->
                    let extractId tree =
                        match tree with
                        | Node (treeEntity, _) -> treeEntity.id
                        | Leaf treeEntity -> treeEntity.id
                    (treeEntity.id, List.map extractId children)
                | Leaf treeEntity ->
                    (treeEntity.id, List.empty)
            Tree.collect collectRelations tree
            |> Map.ofList
        {
            rootId = rootId
            entities = entityTable
            relations = relationsTable
        }
