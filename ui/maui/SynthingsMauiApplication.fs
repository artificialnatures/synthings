namespace synthings.ui.maui

module SynthingsMauiApplication =
    open synthings.transmission
    
    let build () =
        MauiApplicationEntryPoint.start ()
        |> ignore
        let renderer = MauiRenderer.create ()
        let configuration =
            {
                messagingImplementation = Channels
            }
        Application(configuration, renderer)
