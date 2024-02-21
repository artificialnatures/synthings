namespace synthings.sandbox.maui

open synthings.ui.maui
open synthings.transmission

module Sandbox =
    [<EntryPoint>]
    let start arguments =
        let application = SynthingsMauiApplication()
        application.Start ()
        |> ignore
        let goodbyeState =
            Node (VerticalStack, [
                Leaf (Text "Goobye, world!")
            ])
        let helloState =
            Node (ApplicationContainer, [
                Node (VerticalStack, [
                    Leaf (Text "Hello, world!")
                    Leaf (Button ("OK", Replace {entityToReplace=Ancestor 1; replacement=goodbyeState}))
                ])
            ])
        let initialProposal =
            Initialize {initialTree = helloState}
        application.Application.Run ()
        application.Application.Enqueue Identifier.empty initialProposal
        0
