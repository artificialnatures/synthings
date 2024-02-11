module ChangeSet

open Expecto
open synthings.transmission

[<Tests>]
let tests =
    testList "ChangeSet" [
        testCase "Initialize proposal" <| fun _ ->
            let entityTable = EntityTable.fromTree (Node (1, [Leaf 10; Node (11, [Leaf 110; Leaf 111])]))
            let tree =
                Node (2, [
                    Leaf 3
                    Node (4, [
                        Leaf 41
                        Leaf 42
                    ])
                ])
            let proposal =
                Initialize {
                    initialTree = tree
                }
            let validateOperation operation =
                match operation with
                | Create operation -> ("Create", operation.entity)
                | Parent _ -> ("Parent", -1)
                | Reorder _ -> ("Invalid", -1)
                | Update _ -> ("Invalid", -1)
                | Orphan _ -> ("Orphan", -1)
                | Delete operation -> ("Delete", operation.entity)
            let actual = 
                match ChangeSet.assemble entityTable proposal with
                | Ok changeSet ->
                    List.map validateOperation changeSet
                | Error message ->
                    failtest message
            let expected = 
                [
                    ("Orphan", -1)
                    ("Delete", 111)
                    ("Orphan", -1)
                    ("Delete", 110)
                    ("Orphan", -1)
                    ("Delete", 11)
                    ("Orphan", -1)
                    ("Delete", 10)
                    ("Delete", 1)  //root entity has no parent, so doesn't require an Orphan operation
                    ("Create", 2)  //root entity has no parent, so doesn't require an Parent operation
                    ("Create", 3)
                    ("Parent", -1)
                    ("Create", 4)
                    ("Parent", -1)
                    ("Create", 41)
                    ("Parent", -1)
                    ("Create", 42)
                    ("Parent", -1)
                ]
            Expect.equal actual expected "ChangeSet should contain delete operations for existing entities and create operations for all new entities in order."

        testCase "Add proposal" <| fun _ ->
            let entityTable = EntityTable.fromTree (Leaf 1)
            let tree =
                Node (2, [
                    Leaf 3
                    Node (4, [
                        Leaf 41
                        Leaf 42
                    ])
                ])
            let proposal =
                Add {
                    parent = Specified entityTable.rootId
                    order = Last
                    entityToAdd = tree
                }
            let validateOperation operation =
                match operation with
                | Create operation -> ("Create", operation.entity)
                | Parent _ -> ("Parent", -1)
                | Reorder _ -> ("Invalid", -1)
                | Update _ -> ("Invalid", -1)
                | Orphan _ -> ("Invalid", -1)
                | Delete _ -> ("Invalid", -1)
            let actual = 
                match ChangeSet.assemble entityTable proposal with
                | Ok changeSet ->
                    List.map validateOperation changeSet
                | Error message ->
                    failtest message
            let expected = 
                [
                    ("Create", 2)
                    ("Parent", -1)
                    ("Create", 3)
                    ("Parent", -1)
                    ("Create", 4)
                    ("Parent", -1)
                    ("Create", 41)
                    ("Parent", -1)
                    ("Create", 42)
                    ("Parent", -1)
                ]
            Expect.equal actual expected "ChangeSet should contain create operations for all entities in order."
        
        testCase "Replace proposal" <| fun _ ->
            let initialTree =
                Node (2, [
                    Leaf 21
                    Node (22, [
                        Leaf 220
                    ])
                    Node (23, [
                        Node (230, [
                            Leaf 2301
                            Leaf 2302
                        ])
                        Leaf 231
                    ])
                ])
            let entityTable = EntityTable.fromTree initialTree
            let entityId = 
                match Map.tryFindKey (fun _ value -> value = 23) entityTable.entities with
                | Some identifier -> identifier
                | None -> Identifier.empty
            let replacementTree =
                Node (24, [
                    Node (240, [
                        Leaf 241
                        Leaf 242
                    ])
                ])
            let proposal =
                Replace {
                    entityToReplace = Specified entityId
                    replacement = replacementTree
                }
            let validateOperation operation =
                match operation with
                | Create operation -> ("Create", operation.entity)
                | Parent _ -> ("Parent", -1)
                | Reorder _ -> ("Invalid", -1)
                | Update operation -> ("Update", operation.entity)
                | Orphan _ -> ("Orphan", -1)
                | Delete operation -> ("Delete", operation.entity)
            let changeSet =
                match ChangeSet.assemble entityTable proposal with
                | Ok changeSet -> changeSet
                | _ -> List.empty
            let actual = List.map validateOperation changeSet
            let expected = 
                [
                    ("Orphan", -1)
                    ("Delete", 231)
                    ("Orphan", -1)
                    ("Delete", 2302)
                    ("Orphan", -1)
                    ("Delete", 2301)
                    ("Orphan", -1)
                    ("Delete", 230)
                    ("Orphan", -1)
                    ("Delete", 23)
                    ("Create", 24)
                    ("Parent", -1)
                    ("Create", 240)
                    ("Parent", -1)
                    ("Create", 241)
                    ("Parent", -1)
                    ("Create", 242)
                    ("Parent", -1)
                ]
            Expect.equal actual expected "ChangeSet should contain update and create operations for all entities in order."
        
        testCase "Remove proposal" <| fun _ ->
            let tree =
                Node (3, [
                    Node (4, [
                        Leaf 41
                        Node (42, [
                            Leaf 421
                            Leaf 422
                        ])
                    ])
                    Node (5, [
                        Leaf 51
                        Leaf 52
                    ])
                ])
            let entityTable = EntityTable.fromTree tree
            let entityId = 
                match Map.tryFindKey (fun _ value -> value = 4) entityTable.entities with
                | Some identifier -> identifier
                | None -> Identifier.empty
            let proposal = Remove { entityToRemove = Specified entityId }
            let validateOperation operation =
                match operation with
                | Create _ -> ("Invalid", -1)
                | Parent _ -> ("Invalid", -1)
                | Reorder _ -> ("Invalid", -1)
                | Update _ -> ("Invalid", -1)
                | Orphan _ -> ("Orphan", -1)
                | Delete operation -> ("Delete", operation.entity)
            let changeSet =
                match ChangeSet.assemble entityTable proposal with
                | Ok changeSet -> changeSet
                | _ -> List.empty
            let actual = List.map validateOperation changeSet
            let expected = 
                [
                    ("Orphan", -1)
                    ("Delete", 422)
                    ("Orphan", -1)
                    ("Delete", 421)
                    ("Orphan", -1)
                    ("Delete", 42)
                    ("Orphan", -1)
                    ("Delete", 41)
                    ("Orphan", -1)
                    ("Delete", 4)
                ]
            Expect.equal actual expected "ChangeSet should contain delete operations for all entities in order."
        
        testCase "Undo yields a reversed and inverted list of operations" <| fun _ ->
            let id1 = Identifier.create()
            let op1 =
                Create {
                    entityId = id1
                    entity = 1
                }
            let id2 = Identifier.create()
            let op2 =
                Create {
                    entityId = id2
                    entity = 2
                }
            let op3 = 
                Delete {
                    entityId = id2
                    entity = 2
                }
            let id4 = Identifier.create()
            let op4 =
                Create {
                    entityId = id4
                    entity = 3
                }
            let op5 =
                Update {
                    entityId = id4
                    entity = 4
                    priorEntity = 3
                }
            let actual = ChangeSet.undo [op1; op2; op3; op4; op5]
            let expected =
                [
                    Update {entityId = id4; entity = 3; priorEntity = 4}
                    Delete {entityId = id4; entity = 3}
                    Create {entityId = id2; entity = 2}
                    Delete {entityId = id2; entity = 2}
                    Delete {entityId = id1; entity = 1}
                ]
            Expect.equal actual expected "ChangeSet.undo should return the same number of operations, in the opposite order, and with the inverse operation."
        
        testCase "Building ChangeSet fails with error when attempting to delete an absent entity" <| fun _ ->
            let entityTable = EntityTable.fromTree (Leaf 1)
            let invalidIdentifier = Identifier.create()
            let proposal = Remove { entityToRemove = Specified invalidIdentifier }
            let actual = ChangeSet.assemble entityTable proposal
            Expect.isError actual "Invalid identifiers should result in an error."
    ]
