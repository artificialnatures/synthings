namespace synthings.ui.maui

open System
open Microsoft.Maui
open Microsoft.Maui.Hosting

type Program() =
    inherit MauiApplication()

    override this.CreateMauiApp() = MauiInitializer.buildMauiApp ()

module MauiApplicationEntryPoint =
    let start () =
        let app = Program()
        app.Run(args)
        0