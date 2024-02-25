namespace synthings.ui.maui

open Microsoft.Maui.Controls

type MauiApplicationRoot(references : MauiReferences) as this =
    inherit Application()
    //let colors = ResourceDictionary(Source=System.Uri("Resources/Styles/Colors.xaml"))
    //do this.Resources.MergedDictionaries.Add(colors)
    //let styles = ResourceDictionary(Source=System.Uri("Resources/Styles/Styles.xaml"))
    //do this.Resources.MergedDictionaries.Add(styles)
    let startMessage = Label(
            Text="synthings",
            FontFamily="Atkinson",
            FontSize=96,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        )
    let rootContent = ContentView(Content=startMessage)
    let rootPage = ContentPage(Content=rootContent)
    do rootPage.Loaded.Add(fun _ -> references.RootPage <- rootPage)
    do this.MainPage <- rootPage
    do references.App <- this :> Application
    do references.RootContent <- rootContent
and MauiReferences() as this =
    let mutable app : Application = null
    let mutable rootPage : ContentPage = null
    let mutable rootContent : ContentView = null
    member this.IsInitialized = rootContent <> null
    member this.App
        with get () = app
        and set (value) = app <- value
    member this.RootPage
        with get () = rootPage
        and set (value) = rootPage <- value
    member this.RootContent
        with get () = rootContent
        and set (value) = rootContent <- value
    member this.ReplaceContent content =
        rootContent.Content <- content