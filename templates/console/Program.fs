namespace synthings.templates.console

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
        let transmission = SynthingsConsoleApplication.build ()
        let initialProposal =
            Initialize {
                initialTree = helloState
            }
        transmission.Enqueue Identifier.empty initialProposal
        transmission.RunBlocking()
        0