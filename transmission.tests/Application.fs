module Application

open Expecto
open synthings.transmission

let render renderedState _ renderTask =
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
        testCase "Initializing" <| fun _ ->
            let mutable renderedState : RenderTable<int> = Map.empty
            let configuration =
                {
                    messagingImplementation = Channels
                    renderer = render renderedState
                }
            let application = Application(configuration)
            let proposal =
                Initialize {
                    initialTree = (Node (2, [Leaf 21; Leaf 22]))
                }
            application.Step {sender=Identifier.empty; proposal=proposal}
            let actual = application.buildTree ()
            let expected = Node (2, [
                Leaf 21
                Leaf 22
            ])
            Expect.equal actual expected "Trees should be equal."
    ]