module EntityTable

open Expecto
open synthings.transmission

let exampleTree = Node (0, [
        Node (1, [
            Leaf 10
            Leaf 11
            Leaf 12
            Leaf 13
            Leaf 14
            Leaf 15
            Leaf 16
            Leaf 17
            Leaf 18
            Leaf 19
        ])
        Node (2, [for number in 20..29 -> Leaf number])
        Node (3, [for number in 30..39 -> Leaf number])
        Node (4, [for number in 40..49 -> Leaf number])
        Node (5, [for number in 50..59 -> Leaf number])
        Node (6, [for number in 60..69 -> Leaf number])
        Node (7, [for number in 70..79 -> Leaf number])
        Node (8, [for number in 80..89 -> Leaf number])
        Node (9, [for number in 90..99 -> Leaf number])
    ])
let exampleEntityTable = EntityTable.fromTree exampleTree
let exampleKeys = 
    Map.toList exampleEntityTable.entities
    |> List.map (fun (key, value) -> (value, key))
    |> Map.ofList

[<Tests>]
let tests =
    testList "EntityTable" [ 
        testCase "Validate returns error when identifier is missing from an EntityTable" <| fun _ ->
            let mutable entityTable = EntityTable.fromTree (Leaf 1)
            let invalidId = Identifier.create ()
            let result = EntityTable.validate entityTable invalidId
            Expect.isError result "Validation of an identifier not present in EntityTable should fail."

        testCase "Resolve an EntityReference" <| fun _ ->
            let examples =
                [
                    //(relativeTo, reference) should equal ->       key
                    ((Identifier.empty, Root),                      exampleKeys[0]) //0 is the root entity
                    ((exampleKeys[1], Ancestor 1),                  exampleKeys[0]) //0 is the parent of 1
                    ((exampleKeys[5], Ancestor 1),                  exampleKeys[0]) //0 is the parent of 5
                    ((exampleKeys[5], Ancestor 0),                  exampleKeys[5]) //The 0th ancestor is myself
                    ((exampleKeys[5], Ancestor -1),                 exampleKeys[5]) //Negative generations is equivalent to 0th ancestor (self)
                    ((exampleKeys[55], Ancestor 1),                 exampleKeys[5]) //5 is the parent of 55
                    ((exampleKeys[55], Ancestor 2),                 exampleKeys[0]) //0 is the grandparent of 55
                    ((exampleKeys[6], Sibling 2),                   exampleKeys[8]) //8 is the sibling 2 steps subsequent to 6
                    ((exampleKeys[32], Sibling -2),                 exampleKeys[30]) //30 is the sibling 2 steps prior to 32
                    ((exampleKeys[75], Sibling 0),                  exampleKeys[75]) //The 0th sibling is myself
                    ((exampleKeys[81], Sibling -2),                 exampleKeys[80]) //If steps goes beyond the first sibling, the first sibling is used
                    ((exampleKeys[88], Sibling 2),                  exampleKeys[89]) //If steps goes beyond the last sibling, the last sibling is used
                    ((Identifier.empty, Specified exampleKeys[99]), exampleKeys[99]) //A specific Identifier anywhere in the EntityTable
                ]
            let resolve example =
                let (relativeTo, reference), _ = example
                EntityTable.resolveIdentifier exampleEntityTable reference relativeTo
            let results = List.map resolve examples
            let actual = 
                List.map Result.toOption results
                |> List.filter Option.isSome
                |> List.map Option.get
            let expected = List.map snd examples
            Expect.equal actual expected "All Identifiers should match."
        
        testCase "Resolve an EntityReference and its parent" <| fun _ ->
            let examples =
                [
                    //(relativeTo, reference) should yield ->           (entityId,          parentId)
                    ((exampleKeys[3], Ancestor 1),                      (exampleKeys[0],    None)) //0 is the root entity, so it doesn't have a parent
                    ((exampleKeys[55], Ancestor 1),                     (exampleKeys[5],    Some exampleKeys[0])) //0 is the parent of 5, which is the parent of 55
                    ((exampleKeys[66], Sibling 2),                      (exampleKeys[68],   Some exampleKeys[6])) //6 is the parent of 68, which is the sibling 2 steps subsequent to 66
                    ((Identifier.empty, Specified exampleKeys[99]),     (exampleKeys[99],   Some exampleKeys[9])) //9 is the parent of 99
                ]
            let resolve example =
                let (relativeTo, reference), _ = example
                EntityTable.resolveIdentifierAndParent exampleEntityTable reference relativeTo
            let results = List.map resolve examples
            let actual = 
                List.map Result.toOption results
                |> List.filter Option.isSome
                |> List.map Option.get
            let expected = List.map snd examples
            Expect.equal actual expected "All Identifiers should match."
        
        testCase "Resolving insertion point" <| fun _ ->
            let insertedChildId = Identifier.create ()
            let examples =
                [
                    (
                        ((Identifier.empty, Root), insertedChildId, First),
                        [
                            insertedChildId
                            exampleKeys[1]
                            exampleKeys[2]
                            exampleKeys[3]
                            exampleKeys[4]
                            exampleKeys[5]
                            exampleKeys[6]
                            exampleKeys[7]
                            exampleKeys[8]
                            exampleKeys[9]
                        ]
                    )
                    (
                        ((exampleKeys[33], Ancestor 1), exampleKeys[35], First),
                        [
                            exampleKeys[35]
                            exampleKeys[30]
                            exampleKeys[31]
                            exampleKeys[32]
                            exampleKeys[33]
                            exampleKeys[34]
                            exampleKeys[36]
                            exampleKeys[37]
                            exampleKeys[38]
                            exampleKeys[39]
                        ]
                    )
                    (
                        ((exampleKeys[44], Sibling -2), insertedChildId, First),
                        List.empty //only one child entity, so no reordering necessary
                    )
                    (
                        ((Identifier.empty, Specified exampleKeys[5]), exampleKeys[52], First),
                        [
                            exampleKeys[52]
                            exampleKeys[50]
                            exampleKeys[51]
                            exampleKeys[53]
                            exampleKeys[54]
                            exampleKeys[55]
                            exampleKeys[56]
                            exampleKeys[57]
                            exampleKeys[58]
                            exampleKeys[59]
                        ]
                    )
                    (
                        ((Identifier.empty, Specified exampleKeys[5]), insertedChildId, Last),
                        List.empty //child entity appended to the end of the list (Last), so no reordering necessary
                    )
                    (
                        ((Identifier.empty, Specified exampleKeys[5]), exampleKeys[59], Precede exampleKeys[55]),
                        [
                            exampleKeys[50]
                            exampleKeys[51]
                            exampleKeys[52]
                            exampleKeys[53]
                            exampleKeys[54]
                            exampleKeys[59]
                            exampleKeys[55]
                            exampleKeys[56]
                            exampleKeys[57]
                            exampleKeys[58]
                        ]
                    )
                ]
            let resolve example =
                let ((relativeTo, parentReference), childId, insertionOrder), _ = example
                let reorderedChildIds = EntityTable.resolveChildOrder exampleEntityTable parentReference relativeTo childId insertionOrder
                match reorderedChildIds with
                | Some (order, _) -> order  //Some means there should be a Reorder operation
                | None -> List.empty        //None means no Reorder operation necessary
            let actual = List.map resolve examples
            let expected = List.map snd examples
            Expect.sequenceEqual actual expected "All Identifiers should match."
        
        testCase "Execute Operation: Create with empty table" <| fun _ ->
            let mutable entityTable = EntityTable.empty
            let operation = Create {entityId = Identifier.create (); entity = 1}
            entityTable <- EntityTable.executeOperation entityTable operation
            let expected = Leaf 1
            let actual = EntityTable.buildTree None entityTable
            Expect.equal actual expected "Trees should match."
            
        testCase "Execute Operation: Create and Parent" <| fun _ ->
            let mutable entityTable = {
                rootId = exampleKeys[1]
                entities = [(exampleKeys[1], 1)] |> Map.ofList
                relations = [(exampleKeys[1], List.empty)] |> Map.ofList
            }
            entityTable <-
                Create {entityId = exampleKeys[2]; entity = 2}
                |> EntityTable.executeOperation entityTable
            entityTable <-
                Parent {entityId = exampleKeys[2]; parentId = exampleKeys[1]}
                |> EntityTable.executeOperation entityTable
            let expected = Node (1, [Leaf 2])
            let actual = EntityTable.buildTree None entityTable
            Expect.equal actual expected "Trees should match."
        
        testCase "Execute Operation: Reorder" <| fun _ ->
            let mutable entityTable = {
                rootId = exampleKeys[1]
                entities = [
                    (exampleKeys[1], 1)
                    (exampleKeys[2], 2)
                    (exampleKeys[3], 3)
                    (exampleKeys[4], 4)
                ] |> Map.ofList
                relations = [
                    (exampleKeys[1], [exampleKeys[2]; exampleKeys[3]; exampleKeys[4]])
                    (exampleKeys[2], List.empty)
                    (exampleKeys[3], List.empty)
                    (exampleKeys[4], List.empty)
                ] |> Map.ofList
            }
            entityTable <-
                Reorder {
                    entityId = exampleKeys[1]
                    order = List.rev entityTable.relations[exampleKeys[1]]
                    priorOrder = entityTable.relations[exampleKeys[1]]
                }
                |> EntityTable.executeOperation entityTable
            let expected = Node (1, [
                Leaf 4
                Leaf 3
                Leaf 2
            ])
            let actual = EntityTable.buildTree None entityTable
            Expect.equal actual expected "Trees should match."
        
        testCase "Execute Operation: Update" <| fun _ ->
            let mutable entityTable = {
                rootId = exampleKeys[1]
                entities = [
                    (exampleKeys[1], 1)
                    (exampleKeys[2], 2)
                ] |> Map.ofList
                relations = [
                    (exampleKeys[1], [exampleKeys[2]])
                    (exampleKeys[2], List.empty)
                ] |> Map.ofList
            }
            entityTable <-
                Update {
                    entityId = exampleKeys[2]
                    entity = 3
                    priorEntity = 2 
                }
                |> EntityTable.executeOperation entityTable
            let expected = Node (1, [
                Leaf 3
            ])
            let actual = EntityTable.buildTree None entityTable
            Expect.equal actual expected "Trees should match."
        
        testCase "Execute Operation: Orphan and Delete" <| fun _ ->
            let mutable entityTable = {
                rootId = exampleKeys[1]
                entities = [
                    (exampleKeys[1], 1)
                    (exampleKeys[2], 2)
                ] |> Map.ofList
                relations = [
                    (exampleKeys[1], [exampleKeys[2]])
                    (exampleKeys[2], List.empty)
                ] |> Map.ofList
            }
            entityTable <-
                Orphan {
                    entityId = exampleKeys[2]
                    parentId = exampleKeys[1] 
                }
                |> EntityTable.executeOperation entityTable
            entityTable <-
                Delete {
                    entityId = exampleKeys[2]
                    entity = 2
                }
                |> EntityTable.executeOperation entityTable
            let expected = Leaf 1
            let actual = EntityTable.buildTree None entityTable
            Expect.equal actual expected "Trees should match."
        
        testCase "Apply ChangeSet" <| fun _ ->
            let mutable entityTable = {
                rootId = exampleKeys[1]
                entities = [
                    (exampleKeys[1], 1)
                    (exampleKeys[2], 2)
                ] |> Map.ofList
                relations = [
                    (exampleKeys[1], [exampleKeys[2]])
                    (exampleKeys[2], List.empty)
                ] |> Map.ofList
            }
            let changeSet = [
                Orphan {
                    entityId = exampleKeys[2]
                    parentId = exampleKeys[1] 
                }
                Delete {
                    entityId = exampleKeys[2]
                    entity = 2
                }
                Create {
                    entityId = exampleKeys[3]
                    entity = 3 
                }
                Parent {
                    entityId = exampleKeys[3]
                    parentId = exampleKeys[1] 
                }
            ]
            entityTable <- EntityTable.apply entityTable changeSet
            let expected = Node (1, [Leaf 3])
            let actual = EntityTable.buildTree None entityTable
            Expect.equal actual expected "Trees should match."
        
        testCase "EntityTable.buildTree" <| fun _ -> 
            let rootId = Identifier.create()
            let entityId2 = Identifier.create()
            let entityId3 = Identifier.create()
            let mutable entityTable =
                {
                    rootId = rootId
                    entities =
                        [
                            (rootId, 1)
                            (entityId2, 2)
                            (entityId3, 3)
                        ]
                        |> Map.ofList
                    relations =
                        [
                            (rootId, [ entityId2 ])
                            (entityId2, [ entityId3 ])
                            (entityId3, List.empty)
                        ]
                        |> Map.ofList
                }
            let actual = EntityTable.buildDetailedTree None entityTable
            let expected =
                Node ({entity = 1; id = rootId; parentId = None}, [
                    Node ({entity = 2; id = entityId2; parentId = Some rootId}, [
                        Leaf {entity = 3; id = entityId3; parentId = Some entityId2}
                    ])
                ])
            Expect.equal actual expected "Trees do not match."
        
        testCase "EntityTable.fromTree" <| fun _ ->
            let tree = 
                Node (1, [
                    Leaf 2
                    Leaf 3
                    Node (4, [
                        Leaf 41
                        Leaf 42
                    ])
                ])
            let entityTable = EntityTable.fromTree tree
            let actualEntities = List.ofSeq entityTable.entities.Values
            let actualRelations = 
                Map.toList entityTable.relations
                |> List.map (fun (parentId, childIds) -> List.map (fun childId -> (parentId, childId)) childIds)
                |> List.concat
                |> List.map (fun (parentId, childId) -> (entityTable.entities[parentId], entityTable.entities[childId]))
            let expectedEntities = [1; 2; 3; 4; 41; 42]
            let expectedRelations =
                [
                    (1, 2)
                    (1, 3)
                    (1, 4)
                    (4, 41)
                    (4, 42)
                ]
            Expect.containsAll actualEntities expectedEntities "entity lists should match"
            Expect.containsAll actualRelations expectedRelations "relations lists should match"
    ]
