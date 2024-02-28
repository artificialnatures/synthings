namespace synthings.sandbox.maui

open Microsoft.Extensions.DependencyInjection
open Microsoft.Maui.Hosting
open Microsoft.Maui.Controls.Hosting
open synthings.ui.maui

type MauiProgram =
    static member CreateMauiApp() =
        let mauiApp =
            MauiApp
                .CreateBuilder()
                .UseMauiApp<SynthingsMauiApplication>()
        mauiApp.Services.AddSingleton(Sandbox.build ())
        |> ignore
        mauiApp.ConfigureFonts(fun fonts ->
                fonts
                    .AddFont("BerkeleyMono-Regular.ttf", "BerkeleyMono")
                    .AddFont("BerkeleyMono-Bold.ttf", "BerkeleyMonoBold")
                    .AddFont("BerkeleyMono-Italic.ttf", "BerkeleyMonoItalic")
                    .AddFont("BerkeleyMono-BoldItalic.ttf", "BerkeleyMonoBoldItalic")
                |> ignore
            )
            |> ignore
        mauiApp.Build()