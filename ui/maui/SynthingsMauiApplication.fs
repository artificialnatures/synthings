namespace synthings.ui.maui

open synthings.transmission

type SynthingsMauiApplication(application : Application<StateRepresentation>) as mauiApp =
    inherit Microsoft.Maui.Controls.Application()
    let startMessage = Microsoft.Maui.Controls.Label(
            Text="waiting",
            FontFamily="Atkinson",
            FontSize=96,
            VerticalOptions = Microsoft.Maui.Controls.LayoutOptions.Center,
            HorizontalOptions = Microsoft.Maui.Controls.LayoutOptions.Center
        )
    let rootContent = Microsoft.Maui.Controls.ContentView(Content=startMessage)
    let rootPage = Microsoft.Maui.Controls.ContentPage(Content=rootContent)
    do mauiApp.MainPage <- rootPage
    let complete _ =
        startMessage.Text <- "synthings"
    do rootPage.Loaded.Add(complete)
