namespace synthings.ui.maui

open Microsoft.Maui.Controls

type MauiApplicationRoot() as this =
    inherit Application()
    //let colors = ResourceDictionary(Source=System.Uri("Resources/Styles/Colors.xaml"))
    //do this.Resources.MergedDictionaries.Add(colors)
    //let styles = ResourceDictionary(Source=System.Uri("Resources/Styles/Styles.xaml"))
    //do this.Resources.MergedDictionaries.Add(styles)
    let startMessage = Label(Text="synthings", FontSize=48, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center)
    let page = ContentPage(Content=startMessage)
    do this.MainPage <- page
