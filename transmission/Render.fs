namespace synthings.transmission

type RendererImplementation =
    | Testing
    | Console
    | MAUI
(*
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
*)
(* TODO: don't need RenderTask? Just use Operation
type RenderTask<'entity, 'renderable> =
    | CreateView of RenderableTask<'entity>
    | ParentView of RelateTask
    | ReorderView of ReorderTask
    | UpdateView of UpdateTask<'entity, 'renderable>
    | OrphanView of RelateTask
    | DeleteView of RenderableTask<'entity>
*)
type RenderTable<'renderable> =
    Map<Identifier, 'renderable>

type TestRenderableA<'entity> =
    {
        renderedBy : string
        entity : 'entity
    }
type TestRenderableB<'entity> =
    {
        time : System.DateTime
        entity : 'entity
    }
type TestRenderableC<'entity> =
    {
        id : Identifier
        entity : 'entity
    }

module Renderer =
    let create<'entity> rendererImplementation =
        match rendererImplementation with
        | Testing ->
            (fun _ -> ())
        | Console ->
            let mutable renderTable : Map<Identifier, TestRenderableB<'entity>> = Map.empty
            let render operation =
                match operation with
                | Create operation ->
                    renderTable <- Map.add operation.entityId {time=System.DateTime.Now; entity=operation.entity} renderTable
                    ()
                | Parent operation ->
                    ()
                | Reorder operation ->
                    ()
                | Update operation ->
                    ()
                | Orphan operation ->
                    ()
                | Delete operation ->
                    ()
            render
        | MAUI ->
            let mutable renderTable : Map<Identifier, TestRenderableC<'entity>> = Map.empty
            let render operation =
                match operation with
                | Create operation ->
                    renderTable <- Map.add operation.entityId {id=operation.entityId; entity=operation.entity} renderTable
                    ()
                | Parent operation ->
                    ()
                | Reorder operation ->
                    ()
                | Update operation ->
                    ()
                | Orphan operation ->
                    ()
                | Delete operation ->
                    ()
            render
