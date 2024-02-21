﻿namespace synthings.ui.maui

open Android.App
open Microsoft.Maui

[<Application>]
type MainApplication(handle, ownership) =
    inherit MauiApplication(handle, ownership)
    
    override _.CreateMauiApp() = AppDelegate.CreateMauiApp()