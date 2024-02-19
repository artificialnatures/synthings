namespace synthings.sandbox.maui

open synthings.transmission
open synthings.ui.maui

module Sandbox =
    let start () =
        let configuration =
            {
                messagingImplementation = Channels
                renderer = MauiRenderer.render // TODO: pull renderer inside transmission? pass in a Console | MAUI | etc. parameter here...
            }
        let application = Application(configuration)
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
        application.Run ()
        application.Enqueue Identifier.empty initialProposal
        application
