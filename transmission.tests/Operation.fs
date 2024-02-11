module Operation

open Expecto
open synthings.transmission

[<Tests>]
let operationTests =
    testList "Operation" [ 
        testCase "Inverting Create yields Delete" <| fun _ ->
            let entityId = Identifier.create ()
            let entity = 1
            let create =
                Create {
                    entityId = entityId
                    entity = entity
                }
            let actual = Operation.invert create
            let expected =
                Delete {
                    entityId = entityId
                    entity = entity
                }
            Expect.equal actual expected "An inverted Create operation should be a Delete operation."
        
        testCase "Inverting Parent yields Orphan" <| fun _ ->
            let entityId = Identifier.create ()
            let parentId = Identifier.create ()
            let parentOperation =
                Parent {
                    entityId = entityId
                    parentId = parentId
                }
            let actual = Operation.invert parentOperation
            let expected =
                Orphan {
                    entityId = entityId
                    parentId = parentId
                }
            Expect.equal actual expected "An inverted Parent operation should be an Orphan operation."
        
        testCase "Inverting Orphan yields Parent" <| fun _ ->
            let entityId = Identifier.create ()
            let parentId = Identifier.create ()
            let orphanOperation =
                Orphan {
                    entityId = entityId
                    parentId = parentId
                }
            let actual = Operation.invert orphanOperation
            let expected =
                Parent {
                    entityId = entityId
                    parentId = parentId
                }
            Expect.equal actual expected "An inverted Orphan operation should be an Parent operation."
        
        testCase "Inverting Reorder yields Reorder" <| fun _ ->
            let childOne = Identifier.create ()
            let childTwo = Identifier.create ()
            let childThree = Identifier.create ()
            let parentId = Identifier.create ()
            let reorderOperation =
                Reorder {
                    entityId = parentId
                    order = [ childThree; childTwo; childOne ]
                    priorOrder = [ childOne; childTwo; childThree ] 
                }
            let actual = Operation.invert reorderOperation
            let expected =
                Reorder {
                    entityId = parentId
                    order = [ childOne; childTwo; childThree ]
                    priorOrder = [ childThree; childTwo; childOne ]
                }
            Expect.equal actual expected "An inverted Reorder operation should swap order and prior order."
        
        testCase "Inverting Update yields Update" <| fun _ ->
            let entityId = Identifier.create()
            let entity = 2
            let priorEntity = 1
            let update = 
                Update {
                    entityId = entityId 
                    entity = entity
                    priorEntity = priorEntity
                }
            let actual = Operation.invert update
            let expected =
                Update {
                    entityId = entityId
                    entity = priorEntity
                    priorEntity = entity
                }
            Expect.equal actual expected "An inverted Update should have it's entity and priorEntity swapped."
        
        testCase "Inverting Delete yields Create" <| fun _ ->
            let entityId = Identifier.create()
            let entity = 1
            let delete =
                Delete {
                    entityId = entityId
                    entity = entity
                }
            let actual = Operation.invert delete
            let expected =
                Create {
                    entityId = entityId
                    entity = entity
                }
            Expect.equal actual expected "A test failed."
    ]
