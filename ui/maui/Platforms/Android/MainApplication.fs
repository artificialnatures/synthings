namespace synthings.ui.maui

open Android.App
open Microsoft.Maui

[<Application>]
type MainApplication(handle, ownership) =
    inherit MauiApplication(handle, ownership)
    
    override _.CreateMauiApp() = MauiInitializer.BuildMauiApp ()

// TODO: Figure out how to start an Android app imperatively
module MauiApplicationEntryPoint =
    let start () =
        0
