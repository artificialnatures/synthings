module Application

open Expecto
open synthings.transmission

type IntegerRenderable =
    {
        value : int
        onActivated : Proposal<int> option
    }

type TestRenderable =
    | Integer of IntegerRenderable

let mutable proposalQueue : list<unit -> unit> = List.empty

let triggerEvents () = //for simulating interaction, e.g. mouse clicks
    List.iter (fun proposal -> proposal ()) proposalQueue
    proposalQueue <- List.empty

let render submitProposal renderTask =
    match renderTask with
    | CreateView task -> Some task.entity
    | ParentView task -> None
    | ReorderView task -> None
    | UpdateView task -> Some task.entity
    | OrphanView task -> None
    | DeleteView task -> None

[<Tests>]
let tests =
    testList "Application" [ 
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
            application.Step {sender=Identifier.empty; proposal=proposal}
            let actual =
                application.buildTree ()
                |> Tree.map (fun renderable -> match renderable with Integer integer -> integer.value)
            let expected = Node (2, [
                Leaf 21
                Leaf 22
            ])
            Expect.equal actual expected "Trees should be equal."
    ]