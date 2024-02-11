namespace synthings.transmission

type RenderableTask<'entity> =
    {
        renderableId : Identifier
        entity : 'entity
    }

type RelateTask =
    {
        parentRenderableId : Identifier
        childRenderableId : Identifier
    }

type ReorderTask =
    {
        parentRenderableId : Identifier
        childRenderableIds : Identifier list
    }

type UpdateTask<'entity, 'renderable> =
    {
        renderable : 'renderable
        renderableId : Identifier
        entity : 'entity
    }

type RenderTask<'entity, 'renderable> =
    | CreateView of RenderableTask<'entity>
    | ParentView of RelateTask
    | ReorderView of ReorderTask
    | UpdateView of UpdateTask<'entity, 'renderable>
    | OrphanView of RelateTask
    | DeleteView of RenderableTask<'entity>

type RenderTable<'renderable> =
    Map<Identifier, 'renderable>

type Renderer<'entity, 'renderable> = 
    MessageDispatcher<Proposal<'entity>> -> RenderTask<'entity, 'renderable> -> 'renderable option

module Renderer =
    let buildRenderTask (renderTable : RenderTable<_>) (operation : Operation<_>) : RenderTask<_, _> =
        match operation with
        | Create operation ->
            CreateView {
                renderableId = operation.entityId
                entity = operation.entity
            }
        | Parent operation ->
            ParentView {
                parentRenderableId = operation.parentId
                childRenderableId = operation.entityId
            }
        | Reorder operation ->
            ReorderView {
                parentRenderableId = operation.entityId
                childRenderableIds = operation.order
            }
        | Update operation ->
            UpdateView {
                renderable = renderTable[operation.entityId]
                renderableId = operation.entityId
                entity = operation.entity
            }
        | Orphan operation ->
            OrphanView {
                parentRenderableId = operation.parentId
                childRenderableId = operation.entityId
            }
        | Delete operation ->
            DeleteView {
                renderableId = operation.entityId
                entity = operation.entity
            }
    
    let renderChangeSet submitProposal renderer (renderTable : RenderTable<_>) (changeSet : ChangeSet<_>) : RenderTable<_> =
        let build = buildRenderTask renderTable
        let renderTasks = List.map build changeSet
        let render renderTable task =
            let renderable = renderer submitProposal task
            match task with
            | CreateView create ->
                match renderable with
                | Some renderable ->
                    Map.add create.renderableId renderable renderTable
                | None -> renderTable
            | ParentView _ -> renderTable
            | ReorderView _ -> renderTable
            | UpdateView update ->
                match renderable with
                | Some renderable ->
                    Map.remove update.renderableId renderTable
                    |> Map.add update.renderableId renderable
                | None -> renderTable
            | OrphanView _ ->
                renderTable
            | DeleteView delete ->
                Map.remove delete.renderableId renderTable
        List.fold render renderTable renderTasks