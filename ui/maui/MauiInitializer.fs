namespace synthings.ui.maui

module MauiInitializer =
    open Microsoft.Maui.Hosting
    open Microsoft.Maui.Controls.Hosting
    
    let buildMauiApp () =
        MauiApp
            .CreateBuilder()
            .UseMauiApp<MauiApplicationRoot>()
            .ConfigureFonts(fun fonts ->
                fonts
                    .AddFont("OpenSans-Regular.ttf", "OpenSansRegular")
                    .AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold")
                    .AddFont("Atkinson-Hyperlegible-Regular-102.ttf", "Atkinson")
                    .AddFont("Atkinson-Hyperlegible-Bold-102.ttf", "AtkinsonBold")
                    .AddFont("Atkinson-Hyperlegible-Italic-102.ttf", "AtkinsonItalic")
                    .AddFont("Atkinson-Hyperlegible-BoldItalic-102.ttf", "AtkinsonBoldItalic")
                |> ignore
            )
            .Build()
