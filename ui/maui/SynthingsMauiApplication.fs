namespace synthings.ui.maui

open Microsoft.FSharp.Control

module SynthingsMauiApplication =
    open synthings.transmission
    
    let waitForMauiToLoad = MauiReferences.Completion.Task
    
    let startMauiApp () =
        MauiApplicationEntryPoint.start ()
        |> ignore
    
    let buildSynthingsApp mauiReferences =
        let configuration =
            {
                messagingImplementation = Channels
            }
        let createRenderer = MauiRenderer.create mauiReferences
        Application(configuration, ApplicationContainer, createRenderer)
    
    let build () =
        startMauiApp ()
        Some MauiReferences.Completion.Task.Result
        |> buildSynthingsApp
