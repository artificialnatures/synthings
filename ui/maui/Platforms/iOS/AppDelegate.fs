namespace synthings.ui.maui

open Foundation
open Microsoft.Maui

[<Register("AppDelegate")>]
type AppDelegate() =
    inherit MauiUIApplicationDelegate()

    override _.CreateMauiApp() = MauiInitializer.buildMauiApp ()

module MauiApplicationEntryPoint =
    open UIKit
    let start () =
        UIApplication.Main(Array.empty, null, typeof<AppDelegate>)
        0
