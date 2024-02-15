module Application

open Expecto
open synthings.transmission

type IntegerRenderable =
    {
        value : int
        onActivated : Proposal<TestRenderable> option
    }
and TestRenderable =
    | Integer of IntegerRenderable

let mutable proposalQueue : list<unit -> unit> = List.empty

let triggerEvents () = //for simulating interaction, e.g. mouse clicks
    List.iter (fun proposal -> proposal ()) proposalQueue
    proposalQueue <- List.empty

let render submitProposal renderTask =
    match renderTask with
    | CreateView task ->
        match task.entity with
        | Integer renderable ->
            match renderable.onActivated with
            | Some proposal ->
                proposalQueue <- List.append proposalQueue [(fun () -> submitProposal task.renderableId proposal)]
            | None -> ()
        Some task.entity
    | ParentView task -> None
    | ReorderView task -> None
    | UpdateView task -> Some task.entity
    | OrphanView task -> None
    | DeleteView task -> None

[<Tests>]
let tests =
    testSequenced <| testList "Application" [ 
        testCase "Initialize" <| fun _ ->
            let configuration =
                {
                    messagingImplementation = Channels
                    renderer = render
                }
            let application = Application(configuration)
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
            let configuration =
                {
                    messagingImplementation = Channels
                    renderer = render
                }
            let application = Application(configuration)
            let proposal =
                let entityToAdd = Leaf (Integer {value=21; onActivated=None})
                let proposal = Add {parent=Self; order=Last; entityToAdd=entityToAdd}
                Initialize {
                    initialTree = Leaf (Integer {value=2;onActivated=Some proposal})
                }
            application.Enqueue Identifier.empty proposal //Initialize application with some state
            application.Step ()
            triggerEvents () //simulate interaction, e.g. a mouse click
            application.Step ()
            let actual =
                application.BuildTree ()
                |> Tree.map (fun renderable -> match renderable with Integer integer -> integer.value)
            let expected = Node (2, [
                Leaf 21
            ])
            Expect.equal actual expected "A new entity should have been added."
        
        testCase "Replace" <| fun _ ->
            let configuration =
                {
                    messagingImplementation = Channels
                    renderer = render
                }
            let application = Application(configuration)
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
            triggerEvents () //simulate interaction, e.g. a mouse click
            application.Step ()
            let actual =
                application.BuildTree ()
                |> Tree.map (fun renderable -> match renderable with Integer integer -> integer.value)
            let expected = Node (3, [
                Leaf 31
                Leaf 32
            ])
            Expect.equal actual expected "The root entity should have been replaced."
    ]