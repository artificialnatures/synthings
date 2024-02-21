namespace synthings.ui.maui

open synthings.transmission

type SynthingsMauiApplication() =
    let configuration =
        {
            messagingImplementation = Channels
        }
    let renderer = MauiRenderer.create ()
    let application = Application(configuration, renderer)
    
    member this.Application = application
    
    member this.Start () =
        MauiApplicationEntryPoint.start ()
