namespace synthings.sandbox.maui

open synthings.transmission

module Sandbox =
    let goodbyeState =
        Node (VerticalStack, [
            Leaf (Text "Goobye, world!")
        ])
    let helloState =
        Node (ApplicationContainer, [
            Node (Window {title = "sandbox"}, [
                Node (VerticalStack, [
                    Leaf (Text "Hello, world!")
                    Leaf (Button ("OK", Replace {entityToReplace=Ancestor 1; replacement=goodbyeState}))
                ])
            ])
        ])
    let initialProposal =
        Initialize {initialTree = helloState}
    
    let build () =
        let transmission = Transmission()
        transmission.Enqueue Identifier.empty initialProposal
        transmission
