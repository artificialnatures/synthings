namespace synthings.ui.maui

module SynthingsMauiApplication =
    open synthings.transmission
    
    let build () =
        MauiApplicationEntryPoint.start ()
        |> ignore
        let configuration =
            {
                messagingImplementation = Channels
            }
        Application(configuration, ApplicationContainer, MauiRenderer.create)
