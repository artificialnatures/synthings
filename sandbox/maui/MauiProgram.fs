namespace synthings.sandbox.maui

open Microsoft.Maui.Hosting
open Microsoft.Maui.Controls.Hosting
open synthings.ui.maui

type MauiProgram =
    static member CreateMauiApp() =
        MauiApp
            .CreateBuilder()
            .UseMauiApp<SynthingsMauiApplication>()
            .ConfigureFonts(fun fonts ->
                fonts
                    .AddFont("BerkeleyMono-Regular.ttf", "BerkeleyMono")
                    .AddFont("BerkeleyMono-Bold.ttf", "BerkeleyMonoBold")
                    .AddFont("BerkeleyMono-Italic.ttf", "BerkeleyMonoItalic")
                    .AddFont("BerkeleyMono-BoldItalic.ttf", "BerkeleyMonoBoldItalic")
                |> ignore
            )
            .Build()