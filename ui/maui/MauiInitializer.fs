namespace synthings.ui.maui

module MauiInitializer =
    open Microsoft.Maui.Hosting
    open Microsoft.Maui.Controls.Hosting
    open Microsoft.Maui.Controls
    
    type MauiApplicationRoot() as this =
        inherit Application()
        //let colors = ResourceDictionary(Source=System.Uri("Resources/Styles/Colors.xaml"))
        //do this.Resources.MergedDictionaries.Add(colors)
        //let styles = ResourceDictionary(Source=System.Uri("Resources/Styles/Styles.xaml"))
        //do this.Resources.MergedDictionaries.Add(styles)
        let startMessage = Label(Text="synthings", FontFamily="Atkinson", FontSize=96, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center)
        let page = ContentPage(Content=startMessage)
        do this.MainPage <- page
        
        member this.ReplaceRootContent replacement =
            match this.MainPage with
            | :? ContentPage as page ->
                page.Content <- replacement
            | _ -> ()
    
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
