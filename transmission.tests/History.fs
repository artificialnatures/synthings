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
                let message = {sender=entityTable.rootId; proposal=proposals[0]}
                match ChangeSet.assemble entityTable message with
                | Ok changeSet -> List.singleton changeSet
                | Error _ -> List.empty
            let revisedHistory = 
                let message = {sender=entityTable.rootId; proposal=proposals[1]}
                match ChangeSet.assemble entityTable message with
                | Ok changeSet -> changeSet :: expected
                | Error _ -> expected
            let _, actual = History.undo revisedHistory
            Expect.equal actual expected "Undo should remove a ChangeSet from History."
    ]
