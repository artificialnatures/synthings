namespace synthings.ui.maui

open Microsoft.Maui.Controls

type MauiApplicationRoot() as this =
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
    let mutable isInitialized = false
    do rootPage.Loaded.Add(fun _ -> isInitialized <- true)
    do this.MainPage <- rootPage
    member this.RootPage = rootPage
    member this.RootContent = rootContent
    member this.IsInitialized = isInitialized
