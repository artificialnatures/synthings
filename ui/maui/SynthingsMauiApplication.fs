namespace synthings.ui.maui

module SynthingsMauiApplication =
    open synthings.transmission
    
    let startMaui () =
        let start () =
            MauiApplicationEntryPoint.start ()
            |> ignore
        Microsoft.Maui.ApplicationModel.MainThread.InvokeOnMainThreadAsync(start)
        |> Async.AwaitTask
        |> Async.RunSynchronously
    
    let build () =
        (*
        let mauiApp =
            match Microsoft.Maui.Controls.Application.Current with
            | :? MauiApplicationRoot as mauiApp -> Some mauiApp
            | _ -> None
        *)
        let configuration =
            {
                messagingImplementation = Channels
            }
        let createRenderer = MauiRenderer.create MauiInitializer.References
        Application(configuration, ApplicationContainer, createRenderer)
