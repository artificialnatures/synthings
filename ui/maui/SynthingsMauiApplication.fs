namespace synthings.ui.maui

module SynthingsMauiApplication =
    open synthings.transmission
    
    let startMaui () =
        MauiApplicationEntryPoint.start ()
        |> ignore
    
    let build () =
        let configuration =
            {
                messagingImplementation = Channels
            }
        let createRenderer = MauiRenderer.create None
        Application(configuration, ApplicationContainer, createRenderer)
