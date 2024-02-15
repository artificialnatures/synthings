module Tree

open Expecto
open synthings.transmission

type NeedlesslyWrappedInteger = {integerValue : int}

[<Tests>]
let tests =
    testList "Tree" [
        testCase "Map" <| fun _ ->
            let tree =
                Node({integerValue=1}, [
                    Leaf {integerValue=2}
                    Node({integerValue=3}, [
                        Leaf {integerValue=31}
                        Leaf {integerValue=32}
                        Leaf {integerValue=33}
                    ])
                    Node({integerValue=4}, [
                        Leaf {integerValue=41}
                        Leaf {integerValue=42}
                        Node ({integerValue=430}, [
                            Leaf {integerValue=431}
                            Leaf {integerValue=432}
                            Leaf {integerValue=433}
                        ])
                    ])
                ])
            let expected =
                Node(1, [
                    Leaf 2
                    Node(3, [
                        Leaf 31
                        Leaf 32
                        Leaf 33
                    ])
                    Node(4, [
                        Leaf 41
                        Leaf 42
                        Node (430, [
                            Leaf 431
                            Leaf 432
                            Leaf 433
                        ])
                    ])
                ])
            let actual = Tree.map (fun wrapped -> wrapped.integerValue) tree
            Expect.equal actual expected "After map, the tree should have the same structure, but different values."

        testCase "Collect" <| fun _ ->
            let tree =
                Node(1, [
                    Leaf 2
                    Node(3, [
                        Leaf 31
                        Leaf 32
                        Leaf 33
                    ])
                    Node(4, [
                        Leaf 41
                        Leaf 42
                        Node (430, [
                            Leaf 431
                            Leaf 432
                            Leaf 433
                        ])
                    ])
                ])
            let collector _ tree =
                let scalar = 10
                let entity =
                    match tree with
                    | Node (entity, _) -> entity
                    | Leaf entity -> entity
                entity * scalar
            let actual = Tree.collect collector tree
            let expected =
                [
                    10
                    20
                    30
                    310
                    320
                    330
                    40
                    410
                    420
                    4300
                    4310
                    4320
                    4330
                ]
            Expect.equal actual expected "Collection should contain all entities in order."
        
        testCase "Collect root and branches" <| fun _ ->
            let tree =
                Node(1, [
                    Leaf 2
                    Node(3, [
                        Leaf 31
                        Leaf 32
                        Leaf 33
                    ])
                    Node(4, [
                        Leaf 41
                        Leaf 42
                        Node (430, [
                            Leaf 431
                            Leaf 432
                            Leaf 433
                        ])
                    ])
                ])
            let collector recursionLevel tree =
                //recursionLevel indicates whether this is the root (FirstPass) 
                //or branches (Default) of the tree
                let scalar =
                    match recursionLevel with
                    | FirstPass -> 10000
                    | Default -> 10
                let entity =
                    match tree with
                    | Node (entity, _) -> entity
                    | Leaf entity -> entity
                entity * scalar
            let actual = Tree.collect collector tree
            let expected =
                [
                    10000
                    20
                    30
                    310
                    320
                    330
                    40
                    410
                    420
                    4300
                    4310
                    4320
                    4330
                ]
            Expect.equal actual expected "Root and branch results should defer according to recursionLevel (FirstPass vs. Default)."
        testCase "Identify assigns entity identifiers" <| fun _ ->
            let tree =
                Node(1, [
                    Leaf 2
                    Node(3, [
                        Leaf 31
                        Leaf 32
                        Leaf 33
                    ])
                    Node(4, [
                        Leaf 41
                        Leaf 42
                        Node (430, [
                            Leaf 431
                            Leaf 432
                            Leaf 433
                        ])
                    ])
                ])
            let collector _ tree =
                match tree with
                | Node (treeEntity, _) -> Some treeEntity.id
                | Leaf treeEntity -> Some treeEntity.id
            let identifiedTree = Tree.identify None None tree
            let actual =
                Tree.collect collector identifiedTree
                |> List.sumBy (fun identifier -> if Option.isSome identifier then 1 else 0)
            let expected = 13
            Expect.equal actual expected "Each entity in a tree should receive an Identifier."
        testCase "Identify assigns parent identifiers" <| fun _ ->
            let tree =                  //indices after Tree.collect
                Node(1, [               //0
                    Leaf 2              //1
                    Node(3, [           //2
                        Leaf 31         //3
                        Leaf 32         //4
                        Leaf 33         //5
                    ])
                    Node(4, [           //6
                        Leaf 41         //7
                        Leaf 42         //8
                        Node (430, [    //9
                            Leaf 431    //10
                            Leaf 432    //11
                            Leaf 433    //12
                        ])
                    ])
                ])
            let rootId = Identifier.create()
            let collectParentId _ tree =
                match tree with
                | Node (treeEntity, _) -> treeEntity.parentId
                | Leaf treeEntity -> treeEntity.parentId
            let collectEntityId _ tree =
                match tree with
                | Node (treeEntity, _) -> Some treeEntity.id
                | Leaf treeEntity -> Some treeEntity.id
            let identifiedTree = Tree.identify (Some rootId) None tree
            let actual = Tree.collect collectParentId identifiedTree
            let entityIds = Tree.collect collectEntityId identifiedTree
            let expected =
                [
                    Some rootId
                    entityIds[0]
                    entityIds[0]
                    entityIds[2]
                    entityIds[2]
                    entityIds[2]
                    entityIds[0]
                    entityIds[6]
                    entityIds[6]
                    entityIds[6]
                    entityIds[9]
                    entityIds[9]
                    entityIds[9]
                ]
            Expect.equal actual expected "Each entity in a tree should have a valid parent."
        testCase "Identify assigns order relative to siblings" <| fun _ ->
            let tree =                  //indices after Tree.collect
                Node(1, [               //0
                    Leaf 2              //1
                    Node(3, [           //2
                        Leaf 31         //3
                        Leaf 32         //4
                        Leaf 33         //5
                    ])
                    Node(4, [           //6
                        Leaf 41         //7
                        Leaf 42         //8
                        Node (430, [    //9
                            Leaf 431    //10
                            Leaf 432    //11
                            Leaf 433    //12
                        ])
                    ])
                ])
            let rootId = Identifier.create()
            let collectParentId _ tree =
                match tree with
                | Node (treeEntity, _) -> treeEntity.parentId
                | Leaf treeEntity -> treeEntity.parentId
            let collectEntityId _ tree =
                match tree with
                | Node (treeEntity, _) -> Some treeEntity.id
                | Leaf treeEntity -> Some treeEntity.id
            let identifiedTree = Tree.identify (Some rootId) None tree
            let actual = Tree.collect collectParentId identifiedTree
            let entityIds = Tree.collect collectEntityId identifiedTree
            let expected =
                [
                    Some rootId
                    entityIds[0]
                    entityIds[0]
                    entityIds[2]
                    entityIds[2]
                    entityIds[2]
                    entityIds[0]
                    entityIds[6]
                    entityIds[6]
                    entityIds[6]
                    entityIds[9]
                    entityIds[9]
                    entityIds[9]
                ]
            Expect.equal actual expected "Each entity in a tree should have a valid insertion order."
    ]
