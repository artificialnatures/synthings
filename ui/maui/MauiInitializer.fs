namespace synthings.ui.maui

open Microsoft.Maui.Hosting
open Microsoft.Maui.Controls.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging

type MauiInitializer =
    static let mutable references : MauiReferences option = None
    static member References
        with get() = references
    
    static member BuildMauiApp () =
        let builder = MauiApp.CreateBuilder().UseMauiApp<MauiApplicationRoot>()
        builder.Services.AddSingleton<MauiReferences>()
        |> ignore
        builder.ConfigureFonts(fun fonts ->
                fonts
                    .AddFont("OpenSans-Regular.ttf", "OpenSansRegular")
                    .AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold")
                    .AddFont("Atkinson-Hyperlegible-Regular-102.ttf", "Atkinson")
                    .AddFont("Atkinson-Hyperlegible-Bold-102.ttf", "AtkinsonBold")
                    .AddFont("Atkinson-Hyperlegible-Italic-102.ttf", "AtkinsonItalic")
                    .AddFont("Atkinson-Hyperlegible-BoldItalic-102.ttf", "AtkinsonBoldItalic")
                |> ignore
            )
            |> ignore
        builder.Logging.AddConsole()
        |> ignore
        let mauiApp = builder.Build()
        references <-
            match mauiApp.Services.GetService(typeof<MauiReferences>) with
            | :? MauiReferences as refs -> Some refs
            | _ -> None
        mauiApp
