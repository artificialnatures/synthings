module Render

open Expecto
open synthings.transmission

[<Tests>]
let tests =
    testList "Render" [ 
        testCase "renderChangeSet updates views in a render table" <| fun _ ->
            let rootId = Identifier.create()
            let twentyOneId = Identifier.create()
            let twentyTwoId = Identifier.create()
            let mutable renderTable = Map<Identifier, int>[(rootId, 1)]
            let changeSet =
                [
                    Update {entityId = rootId; entity = 2; priorEntity = 1}
                    Create {entityId = twentyOneId; entity = 21}
                    Parent {parentId = rootId; entityId = twentyOneId}
                    Create {entityId = twentyTwoId; entity = 22}
                    Parent {parentId = rootId; entityId = twentyTwoId}
                ]
            let render _ renderTask =
                match renderTask with
                | CreateView create -> Some create.entity
                | ParentView _ -> None
                | ReorderView _ -> None
                | UpdateView update -> Some update.entity
                | OrphanView _ -> None
                | DeleteView _ -> None
            renderTable <- Renderer.renderChangeSet (fun _ -> ()) render renderTable changeSet
            let changeSet =
                [
                    Orphan {parentId = rootId; entityId = twentyTwoId}
                    Delete {entityId = twentyTwoId; entity = 22}
                ]
            renderTable <- Renderer.renderChangeSet (fun _ -> ()) render renderTable changeSet
            let actual = List.ofSeq renderTable.Values
            let expected = [2; 21]
            Expect.containsAll actual expected "All views in the render table should be updated according to the ChangeSet."
    ]
