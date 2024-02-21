namespace synthings.ui.maui

open Foundation
open UIKit
open Microsoft.Maui

[<Register("AppDelegate")>]
type AppDelegate() =
    inherit MauiUIApplicationDelegate()
    
    let mutable mauiApp : Hosting.MauiApp = null
    
    member this.MauiApp = mauiApp
    
    override this.CreateMauiApp() =
        mauiApp <- MauiProgram.CreateMauiApp()
        mauiApp

module MauiApplicationEntryPoint =
    let start () =
        UIApplication.Main(Array.empty, null, typeof<AppDelegate>)
        0