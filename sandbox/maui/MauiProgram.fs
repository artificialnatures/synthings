namespace synthings.sandbox.maui

open synthings.transmission
open Microsoft.Maui.Hosting
open Microsoft.Maui.Controls.Hosting

type MauiView = Microsoft.Maui.Controls.View
type MauiLayout = Microsoft.Maui.Controls.Layout
type MauiCommand = Microsoft.Maui.Controls.Command

module MauiRenderer =
    let createView submitProposal renderableId stateRepresentation =
        match stateRepresentation with
        | ApplicationContainer -> 
            Microsoft.Maui.Controls.ContentView() :> MauiView
        | Cursor -> 
            Microsoft.Maui.Controls.ContentView() :> MauiView
        | Window _ -> 
            Microsoft.Maui.Controls.ContentView() :> MauiView
        | VerticalStack ->
            let stack = Microsoft.Maui.Controls.VerticalStackLayout()
            stack :> MauiView
        | Canvas ->
            let canvas = InfiniteCanvas(stateRepresentation, submitProposal)
            canvas :> MauiView
        | Text text -> 
            Microsoft.Maui.Controls.Label(Text = text, FontSize = 460.0) :> MauiView
        | Button(label, onClickProposal) ->
            let onClick () =
                submitProposal renderableId onClickProposal
            Microsoft.Maui.Controls.Button(Text = label, Command = MauiCommand(onClick)) :> MauiView
        | Image image ->
            match image with
            | DotNetBot ->
                Microsoft.Maui.Controls.Image(
                    Source = Microsoft.Maui.Controls.ImageSource.FromFile("dotnet_bot.png")
                ) :> MauiView
            | Local imagePath ->
                Microsoft.Maui.Controls.Image(Source = Microsoft.Maui.Controls.ImageSource.FromFile(imagePath)) :> MauiView
        | File file ->
            Microsoft.Maui.Controls.Label(
                Text = $"File: Path = {file.filePath}, Size = {file.sizeInBytes} bytes",
                FontSize = 20.0
            ) :> MauiView
        | FilePicker filePaths ->
            let content = Microsoft.Maui.Controls.HorizontalStackLayout()
            content.Children.Add(Microsoft.Maui.Controls.Entry(Placeholder = "File Path"))
            content.Children.Add(Microsoft.Maui.Controls.Button(Text = "Pick File"))
            content :> MauiView
        | Transform(_, _) ->
            Microsoft.Maui.Controls.ContentView() :> MauiView
        | Wait -> 
            Microsoft.Maui.Controls.Label(Text = "Loading...", FontSize = 300.0) :> MauiView
    
    let parent (parentView : MauiView) (childView : MauiView) =
        let addChild =
            match parentView with
            | :? Microsoft.Maui.Controls.ContentView as parent ->
                (fun () -> parent.Content <- childView)
            | :? Microsoft.Maui.Controls.VerticalStackLayout as parent ->
                (fun () -> parent.Children.Add childView)
            | _ -> (fun () -> ())
        addChild ()

type MauiRootContent() as mauiRootContent =
    inherit Microsoft.Maui.Controls.ContentView()

    let renderRoot = Microsoft.Maui.Controls.ContentView(Background = Microsoft.Maui.Controls.SolidColorBrush.LightBlue, MinimumHeightRequest = 500.0)
    let debugRoot = Microsoft.Maui.Controls.ContentView(Background = Microsoft.Maui.Controls.SolidColorBrush.LightYellow, MinimumHeightRequest = 200.0)
    let configuration =
        {
            messagingImplementation = Channels
            renderer = mauiRootContent.Render
        }

    do mauiRootContent.Loaded.Add(mauiRootContent.OnLoaded)

    member mauiRootContent.Application = Application(configuration)

    member mauiRootContent.Render submitProposal renderTask =
        match renderTask with
        | CreateView task -> 
            let renderable = MauiRenderer.createView submitProposal task.renderableId task.entity
            Some renderable
        | ParentView task ->
            let parentView = mauiRootContent.Application.FindRenderable task.parentRenderableId
            let childView = mauiRootContent.Application.FindRenderable task.childRenderableId
            match parentView, childView with
            | Some parentView, Some childView ->
                MauiRenderer.parent parentView childView
            | _ -> ()
            None
        | ReorderView task ->
            None
        | UpdateView task ->
            None
        | OrphanView task ->
            None
        | DeleteView task ->
            None

    member mauiRootContent.OnLoaded eventArgs =
        //Debug Window:
        //let debugPage = Microsoft.Maui.Controls.ContentPage(Content = debugRoot)
        //let debugWindow = Microsoft.Maui.Controls.Window(debugPage)
        //do Microsoft.Maui.Controls.Application.Current.OpenWindow(debugWindow)
        mauiRootContent.Content <- renderRoot
        let goodbyeState =
            Node (VerticalStack, [
                Leaf (Text "Goobye, world!")
            ])
        let helloState =
            Node (ApplicationContainer, [
                Node (VerticalStack, [
                    Leaf (Text "Hello, world!")
                    Leaf (Button ("OK", Replace {entityToReplace=Ancestor 1; replacement=goodbyeState}))
                ])
            ])
        let initialProposal =
            Initialize {initialTree = helloState}
        mauiRootContent.Application.Run ()
        mauiRootContent.Application.Enqueue Identifier.empty initialProposal

    member mauiRootContent.ReplaceRootContent (content: MauiView) =
        Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread((fun () -> renderRoot.Content <- content))
    
    member mauiRootContent.ReplaceDebugContent (content: MauiView) =
        Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread((fun () -> debugRoot.Content <- content))

type MauiMainPage() as mauiPage =
    inherit Microsoft.Maui.Controls.ContentPage()

    do mauiPage.Loaded.Add(mauiPage.OnLoaded)

    member mauiPage.OnLoaded eventArgs =
        mauiPage.Content <- MauiRootContent()

open Microsoft.Extensions.Logging

type MauiApplication() as mauiApp =
    inherit Microsoft.Maui.Controls.Application()
    do mauiApp.MainPage <- MauiMainPage()


type MauiProgram =
    static member CreateMauiApp() =
        let builder = MauiApp.CreateBuilder()
#if DEBUG
        builder.Logging.AddDebug() |> ignore
#endif
        builder.UseMauiApp<MauiApplication>() |> ignore

        builder.ConfigureFonts(fun fonts ->
            fonts
                .AddFont("OpenSans-Regular.ttf", "OpenSansRegular")
                .AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold")
            |> ignore)
        |> ignore

        builder.Build()
