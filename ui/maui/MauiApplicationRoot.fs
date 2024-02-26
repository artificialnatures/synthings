namespace synthings.ui.maui

open Microsoft.Maui.Controls

type MauiReferences() =
    static let completion = System.Threading.Tasks.TaskCompletionSource<MauiReferences>()
    let mutable app : Application = null
    let mutable rootPage : ContentPage = null
    let mutable rootContent : ContentView = null
    static member Completion = completion
    member _.App
        with get () = app
        and set (value) = app <- value
    member _.RootPage
        with get () = rootPage
        and set (value) = rootPage <- value
    member _.RootContent
        with get () = rootContent
        and set (value) = rootContent <- value
    member _.ReplaceContent content =
        rootContent.Content <- content

type MauiApplicationRoot(references : MauiReferences) as mauiApplicationRoot =
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
    do mauiApplicationRoot.MainPage <- rootPage
    let complete () =
        references.App <- mauiApplicationRoot :> Application
        references.RootContent <- rootContent
        references.RootPage <- rootPage
        MauiReferences.Completion.SetResult(references)
        ()
    do rootPage.Loaded.Add(fun _ -> references.RootPage <- rootPage)
