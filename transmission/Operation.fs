namespace synthings.transmission

type EntityOperation<'entity> =
    {
        entityId : Identifier
        entity : 'entity
    }

type RelationOperation<'entity> =
    {
        entityId : Identifier
        parentId : Identifier
    }

type OrderOperation<'entity> =
    {
        entityId : Identifier
        order : Identifier list
        priorOrder : Identifier list
    }

type UpdateOperation<'entity> =
    {
        entityId : Identifier
        entity : 'entity
        priorEntity : 'entity
    }

type Operation<'entity> =
    | Create of EntityOperation<'entity>
    | Parent of RelationOperation<'entity>
    | Reorder of OrderOperation<'entity>
    | Update of UpdateOperation<'entity>
    | Orphan of RelationOperation<'entity>
    | Delete of EntityOperation<'entity>

module Operation =
    let invert operation =
        match operation with
        | Create operation -> 
            Delete {
                entityId = operation.entityId
                entity = operation.entity
            }
        | Parent operation ->
            Orphan {
                entityId = operation.entityId
                parentId = operation.parentId 
            }
        | Reorder operation ->
            Reorder {
                entityId = operation.entityId
                order = operation.priorOrder
                priorOrder = operation.order 
            }
        | Update operation -> 
            Update {
                entityId = operation.entityId
                entity = operation.priorEntity
                priorEntity = operation.entity
            }
        | Orphan operation ->
            Parent {
                entityId = operation.entityId
                parentId = operation.parentId 
            }
        | Delete operation -> 
            Create {
                entityId = operation.entityId
                entity = operation.entity
            }
