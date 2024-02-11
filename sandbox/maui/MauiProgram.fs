namespace synthings.sandbox.maui

open synthings.transmission
open Microsoft.Maui.Hosting
open Microsoft.Maui.Controls.Hosting

type MauiView = Microsoft.Maui.Controls.View
type MauiLayout = Microsoft.Maui.Controls.Layout
type MauiCommand = Microsoft.Maui.Controls.Command

module MauiRenderer =
    let render (submitProposal : MessageDispatcher<Proposal<StateRepresentation>>) (stateRepresentation : StateRepresentation) : MauiView =
        match stateRepresentation with
        | ApplicationContainer _ -> 
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
                submitProposal onClickProposal
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
        | Wait -> 
            Microsoft.Maui.Controls.Label(Text = "Loading...", FontSize = 300.0) :> MauiView

type MauiRootContent() as mauiRootContent =
    inherit Microsoft.Maui.Controls.ContentView()

    let renderRoot = Microsoft.Maui.Controls.ContentView(Background = Microsoft.Maui.Controls.SolidColorBrush.LightBlue, MinimumHeightRequest = 500.0)
    let debugRoot = Microsoft.Maui.Controls.ContentView(Background = Microsoft.Maui.Controls.SolidColorBrush.LightYellow, MinimumHeightRequest = 200.0)
    
    do mauiRootContent.Loaded.Add(mauiRootContent.OnLoaded)

    member mauiRootContent.OnLoaded eventArgs =
        let debugPage = Microsoft.Maui.Controls.ContentPage(Content = debugRoot)
        let debugWindow = Microsoft.Maui.Controls.Window(debugPage)
        do Microsoft.Maui.Controls.Application.Current.OpenWindow(debugWindow)
        mauiRootContent.Content <- renderRoot
        let application = Application(Channels)
        application.Run (Some MauiRenderer.render) //TODO: still need to fully pull render inside application...

        let executeChangeSet (submitProposal : MessageDispatcher<Proposal<StateRepresentation>>) (changeSet : ChangeSet<StateRepresentation>) =
            for operation in changeSet do
                match operation with
                | Create create ->
                    let view = MauiRenderer.render submitProposal create.entity
                    entityIdToView <- Map.add entityId view entityIdToView
                    viewIdToEntityId <- Map.add view.Id entityId viewIdToEntityId
                    //TODO: Use Insert to place the view in order
                    match entityIdToView[parentId] with
                    | :? MauiLayout -> (entityIdToView[parentId] :?> MauiLayout).Add(view)
                | Update (entityId, entity, previousEntity) ->
                    let view = MauiRenderer.render submitProposal entity
                    viewIdToEntityId <- Map.remove entityIdToView[entityId].Id viewIdToEntityId
                    viewIdToEntityId <- Map.add view.Id entityId viewIdToEntityId
                    entityIdToView <- Map.remove entityId entityIdToView
                    entityIdToView <- Map.add entityId view entityIdToView
                | Delete (parentId, childToProceedId, entityId, entity) ->
                    match entityIdToView[parentId] with
                    | :? MauiLayout -> 
                        (entityIdToView[parentId] :?> MauiLayout).Remove(entityIdToView[entityId]) |> ignore
                        viewIdToEntityId <- Map.remove entityIdToView[entityId].Id viewIdToEntityId
                        entityIdToView <- Map.remove entityId entityIdToView

        let render submitProposal changeSet =
            System.Console.WriteLine "Received render task."
            executeChangeSet submitProposal changeSet
            mauiRootContent.ReplaceDebugContent (Microsoft.Maui.Controls.Label(Text = sprintf "Change Set: %A" changeSet))
        Application.runRender render application
        //TODO: Specify initial state, send initial proposals
        

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
