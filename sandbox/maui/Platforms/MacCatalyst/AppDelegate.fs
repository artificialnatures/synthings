namespace synthings.sandbox.maui

open Foundation
open Microsoft.Maui

[<Register("AppDelegate")>]
type AppDelegate() =
    inherit MauiUIApplicationDelegate()
    
    override this.CreateMauiApp() = MauiProgram.CreateMauiApp()