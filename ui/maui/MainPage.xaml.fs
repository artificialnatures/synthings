namespace synthings.ui.maui

open Microsoft.Maui.Controls
open Microsoft.Maui.Controls.Xaml
open System

type MainPage() as this =
    inherit ContentPage()

    do this.LoadFromXaml(typeof<MainPage>) |> ignore
