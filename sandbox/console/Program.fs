namespace synthings.sandbox.console

open synthings.transmission
open synthings.ui.console

module Program =
    [<EntryPoint>]
    let main arguments =
        let goodbyeState =
            Node (
                VerticalStack,
                [
                    Leaf (Text "Goodbye, world!")
                ]
            )
        let helloState =
            Node (
                VerticalStack,
                [
                    Leaf (Text "Hello, world!")
                    Leaf (Button ("OK", Replace {entityToReplace = Ancestor 1; replacement = goodbyeState}))
                ]
            )
        let application = SynthingsConsoleApplication.create ()
        let initialProposal =
            Initialize {
                initialTree = helloState
            }
        application.Enqueue Identifier.empty initialProposal
        application.RunBlocking()
        0