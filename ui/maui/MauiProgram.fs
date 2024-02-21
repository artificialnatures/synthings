namespace synthings.ui.maui

open Microsoft.Maui.Hosting
open Microsoft.Maui.Controls.Hosting

type MauiProgram =
    static member CreateMauiApp () =
        let mauiApp =
            MauiApp
                .CreateBuilder()
                .UseMauiApp<App>()
                .ConfigureFonts(fun fonts ->
                    fonts
                        .AddFont("OpenSans-Regular.ttf", "OpenSansRegular")
                        .AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold")
                    |> ignore
                )
                .Build()
        mauiApp
