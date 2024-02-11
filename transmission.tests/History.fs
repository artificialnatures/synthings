module History

open Expecto
open synthings.transmission

[<Tests>]
let tests =
    testList "History" [ 
        testCase "History" <| fun _ ->
            let entityTable = EntityTable.fromTree (Leaf 1)
            let proposals = 
                [
                    Add {
                        parent = Specified entityTable.rootId
                        order = Last
                        entityToAdd = (Node (2, [Leaf 21]))
                    }
                    Add {
                        parent = Specified entityTable.rootId
                        order = Last
                        entityToAdd = (Node (3, [Leaf 31]))
                    }
                ]
            let expected = 
                match ChangeSet.assemble entityTable proposals[0] with
                | Ok changeSet -> List.singleton changeSet
                | Error _ -> List.empty
            let revisedHistory = 
                match ChangeSet.assemble entityTable proposals[1] with
                | Ok changeSet -> changeSet :: expected
                | Error _ -> expected
            let _, actual = History.undo revisedHistory
            Expect.equal actual expected "Undo should remove a ChangeSet from History."
    ]
