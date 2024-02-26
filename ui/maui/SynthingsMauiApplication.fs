namespace synthings.ui.maui

open synthings.transmission
//TODO: inject - application : Application<StateRepresentation>
type SynthingsMauiApplication() as mauiApp =
    inherit Microsoft.Maui.Controls.Application()
    //let colors = ResourceDictionary(Source=System.Uri("Resources/Styles/Colors.xaml"))
    //do this.Resources.MergedDictionaries.Add(colors)
    //let styles = ResourceDictionary(Source=System.Uri("Resources/Styles/Styles.xaml"))
    //do this.Resources.MergedDictionaries.Add(styles)
    let startMessage = Microsoft.Maui.Controls.Label(
            Text="synthings",
            FontFamily="Atkinson",
            FontSize=96,
            VerticalOptions = Microsoft.Maui.Controls.LayoutOptions.Center,
            HorizontalOptions = Microsoft.Maui.Controls.LayoutOptions.Center
        )
    let rootContent = Microsoft.Maui.Controls.ContentView(Content=startMessage)
    let rootPage = Microsoft.Maui.Controls.ContentPage(Content=rootContent)
    do mauiApp.MainPage <- rootPage
    let complete _ = ()
    do rootPage.Loaded.Add(complete)
