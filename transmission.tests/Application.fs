module Application

open Expecto
open synthings.transmission

type IntegerElement =
    {
        value : int
        onActivated : Proposal<TestState> option
    }
and TestState =
    | Integer of IntegerElement

let triggerEvent submitProposal testState = //for simulating interaction, e.g. mouse clicks
    match testState with
    | Integer integerElement ->
        match integerElement.onActivated with
        | Some proposal -> submitProposal proposal
        | None -> ()

[<Tests>]
let tests =
    //testSequenced ensures that this collection of tests executes
    //serially. These tests hang when run concurrently, likely because
    //of some limitation in System.Threading.Channels
    testSequenced <| testList "Application" [ 
        testCase "Initialize" <| fun _ ->
            let application = Application()
            let proposal =
                Initialize {
                    initialTree = (Node (Integer {value=2;onActivated=None}, [Leaf (Integer {value=21;onActivated=None}); Leaf (Integer {value=22;onActivated=None})]))
                }
            application.Enqueue Identifier.empty proposal
            application.Step ()
            let actual =
                application.BuildTree ()
                |> Tree.map (fun renderable -> match renderable with Integer integer -> integer.value)
            let expected = Node (2, [
                Leaf 21
                Leaf 22
            ])
            Expect.equal actual expected "Trees should be equal."
        
        testCase "Add" <| fun _ ->
            let application = Application()
            let proposal =
                let entityToAdd = Leaf (Integer {value=21; onActivated=None})
                let proposal = Add {parent=Self; order=Last; entityToAdd=entityToAdd}
                Initialize {
                    initialTree = Leaf (Integer {value=2;onActivated=Some proposal})
                }
            application.Enqueue Identifier.empty proposal //Initialize application with some state
            application.Step ()
            application.ForEachEntity triggerEvent //simulate interaction, e.g. a mouse click
            application.Step ()
            let actual =
                application.BuildTree ()
                |> Tree.map (fun renderable -> match renderable with Integer integer -> integer.value)
            let expected = Node (2, [
                Leaf 21
            ])
            Expect.equal actual expected "A new entity should have been added."
        
        testCase "Replace" <| fun _ ->
            let application = Application()
            let replacementTree =
                Node (Integer {value=3; onActivated=None}, [
                    Leaf (Integer {value=31; onActivated=None})
                    Leaf (Integer {value=32; onActivated=None})
                ])
            let replaceProposal =
                Replace {
                    entityToReplace = Ancestor 1 //replace the parent of the sender
                    replacement = replacementTree
                }
            let initialTree =
                Node (Integer {value=2; onActivated=None}, [
                    Leaf (Integer {value=21; onActivated=Some replaceProposal})
                    Leaf (Integer {value=22; onActivated=None})
                ])
            let initialProposal =
                Initialize {
                    initialTree = initialTree
                }
            application.Enqueue Identifier.empty initialProposal //Initialize application with some state
            application.Step ()
            application.ForEachEntity triggerEvent //simulate interaction, e.g. a mouse click
            application.Step ()
            let actual =
                application.BuildTree ()
                |> Tree.map (fun renderable -> match renderable with Integer integer -> integer.value)
            let expected = Node (3, [
                Leaf 31
                Leaf 32
            ])
            Expect.equal actual expected "The root entity should have been replaced."
        
        testCase "Remove" <| fun _ ->
            let application = Application()
            let removeProposal =
                Remove {
                    entityToRemove = Ancestor 1 //remove the parent of the sender
                }
            let initialTree =
                Node (Integer {value=2; onActivated=None}, [
                    Node (Integer {value=21; onActivated=None}, [
                        Leaf (Integer {value=210; onActivated=Some removeProposal})
                        Leaf (Integer {value=211; onActivated=None})
                    ])
                    Leaf (Integer {value=22; onActivated=None})
                ])
            let initialProposal =
                Initialize {
                    initialTree = initialTree
                }
            application.Enqueue Identifier.empty initialProposal //Initialize application with some state
            application.Step ()
            application.ForEachEntity triggerEvent //simulate interaction, e.g. a mouse click
            application.Step ()
            let actual =
                application.BuildTree ()
                |> Tree.map (fun renderable -> match renderable with Integer integer -> integer.value)
            let expected = Node (2, [
                Leaf 22
            ])
            Expect.equal actual expected "An entity should have been deleted."
        
        testCase "Render" <| fun _ ->
            let application = Application()
            let mutable renderTable : Map<Identifier, TestState> = Map.empty
            let render submitProposal operation =
                match operation with
                | Create operation ->
                    let create () =
                        renderTable <- Map.add operation.entityId operation.entity renderTable
                    create
                | Parent operation ->
                    let parent () =
                        let parent = Map.tryFind operation.parentId renderTable
                        let child = Map.tryFind operation.entityId renderTable
                        match parent, child with
                        | Some parent, Some child -> ()
                        | _ -> failwith "Could not find parent and/or child in render table."
                    parent
                | Reorder operation -> (fun () -> ())
                | Update operation -> (fun () -> ())
                | Orphan operation -> (fun () -> ())
                | Delete operation -> (fun () -> ())
            application.WithRenderer render None
            let proposal =
                Initialize {
                    initialTree = (Node (Integer {value=2;onActivated=None}, [Leaf (Integer {value=21;onActivated=None}); Leaf (Integer {value=22;onActivated=None})]))
                }
            application.Enqueue Identifier.empty proposal
            application.Step ()
            let extractValue state =
                match state with
                | Integer state -> state.value
            let collector _ tree =
                let state =
                    match tree with
                    | Node (state, _) -> state
                    | Leaf state -> state
                extractValue state
            let actual =
                application.BuildTree ()
                |> Tree.collect collector
            let expected = [
                2
                21
                22
            ]
            Expect.equal actual expected "Trees should be equal."
    ]