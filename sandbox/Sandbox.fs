namespace synthings.sandbox

open synthings.transmission

module Sandbox =
    let initialState =
        Node (ApplicationContainer, [
            Node (Window {title = "sandbox"}, [
                Node (Canvas, [
                    Leaf (Text "synthings")
                    Leaf (Text "synthings")
                    Leaf (Text "synthings")
                ])
            ])
        ])
    let initialProposal =
        Initialize {initialTree = initialState}
    
    let build () =
        let transmission = Transmission()
        transmission.Enqueue Identifier.empty initialProposal
        transmission
