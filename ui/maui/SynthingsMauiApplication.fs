namespace synthings.ui.maui

open synthings.transmission

type MauiCommand = Microsoft.Maui.Controls.Command
type MauiView =
    | MauiApplication of SynthingsMauiApplication
    | MauiCursor of Microsoft.Maui.Controls.ContentView
    | MauiWindow of Microsoft.Maui.Controls.Window
    | MauiVerticalStack of Microsoft.Maui.Controls.VerticalStackLayout
    | MauiCanvas of InfiniteCanvas
    | MauiText of Microsoft.Maui.Controls.Label
    | MauiButton of Microsoft.Maui.Controls.Button
    | MauiImage of Microsoft.Maui.Controls.Image
    | MauiFilePicker of Microsoft.Maui.Controls.HorizontalStackLayout
    | MauiTransform of Microsoft.Maui.Controls.ContentView
    | MauiWait of Microsoft.Maui.Controls.Label
and SynthingsMauiApplication(application : Application<StateRepresentation>) as mauiApp =
    inherit Microsoft.Maui.Controls.Application()
    
    let createView submitProposal renderableId stateRepresentation =
        match stateRepresentation with
        | ApplicationContainer -> 
            mauiApp |> MauiApplication
        | Cursor -> 
            Microsoft.Maui.Controls.ContentView() |> MauiCursor
        | Window window -> 
            //TODO: Handle multiple windows
            let primaryWindow = mauiApp.Windows.Item 0
            primaryWindow.Title <- window.title
            primaryWindow |> MauiWindow
        | VerticalStack ->
            Microsoft.Maui.Controls.VerticalStackLayout() |> MauiVerticalStack
        | Canvas ->
            InfiniteCanvas(stateRepresentation, submitProposal) |> MauiCanvas
        | Text text -> 
            Microsoft.Maui.Controls.Label(Text = text, FontSize = 460.0) |> MauiText
        | Button(label, onClickProposal) ->
            let onClick () =
                submitProposal renderableId onClickProposal
            Microsoft.Maui.Controls.Button(Text = label, Command = MauiCommand(onClick)) |> MauiButton
        | Image image ->
            match image with
            | DotNetBot ->
                Microsoft.Maui.Controls.Image(
                    Source = Microsoft.Maui.Controls.ImageSource.FromFile("dotnet_bot.png")
                ) |> MauiImage
            | Local imagePath ->
                Microsoft.Maui.Controls.Image(
                    Source = Microsoft.Maui.Controls.ImageSource.FromFile(imagePath)
                ) |> MauiImage
        | File file ->
            Microsoft.Maui.Controls.Label(
                Text = $"File: Path = {file.filePath}, Size = {file.sizeInBytes} bytes",
                FontSize = 20.0
            ) |> MauiText
        | FilePicker filePaths ->
            let content = Microsoft.Maui.Controls.HorizontalStackLayout()
            content.Children.Add(Microsoft.Maui.Controls.Entry(Placeholder = "File Path"))
            content.Children.Add(Microsoft.Maui.Controls.Button(Text = "Pick File"))
            content |> MauiFilePicker
        | Transform _ ->
            Microsoft.Maui.Controls.ContentView() |> MauiTransform
        | Wait -> 
            Microsoft.Maui.Controls.Label(Text = "Loading...", FontSize = 300.0) |> MauiWait
    
    let unbox mauiView =
        match mauiView with
        | MauiApplication view -> None
        | MauiCursor view -> view :> Microsoft.Maui.Controls.View |> Some
        | MauiWindow view -> None
        | MauiVerticalStack view -> view :> Microsoft.Maui.Controls.View |> Some
        | MauiCanvas view -> view :> Microsoft.Maui.Controls.View |> Some
        | MauiText view -> view :> Microsoft.Maui.Controls.View |> Some
        | MauiButton view -> view :> Microsoft.Maui.Controls.View |> Some
        | MauiImage view -> view :> Microsoft.Maui.Controls.View |> Some
        | MauiFilePicker view -> view :> Microsoft.Maui.Controls.View |> Some
        | MauiTransform view -> view :> Microsoft.Maui.Controls.View |> Some
        | MauiWait view -> view :> Microsoft.Maui.Controls.View |> Some
        
    
    let parent parentView childView =
        match parentView with
        | MauiApplication parent -> ()
        | MauiCursor parent -> ()
        | MauiWindow parent ->
            //TODO: handle multiple windows
            match unbox childView with
            | Some child ->
                mauiApp.ReplaceRootContent(child)
            | None -> ()
        | MauiVerticalStack parent ->
            match unbox childView with
            | Some child -> parent.Children.Add child
            | None -> ()
        | MauiCanvas parent -> ()
        | MauiText parent -> ()
        | MauiButton parent -> ()
        | MauiImage parent -> ()
        | MauiFilePicker parent -> ()
        | MauiTransform parent -> ()
        | MauiWait parent -> ()
    
    let createRenderer () =
        let mutable renderTable : Map<Identifier, MauiView> = Map.empty
        let render submitProposal operation =
            match operation with
            | Create operation -> 
                let create () =
                    match Map.tryFind operation.entityId renderTable with
                    | Some renderable ->
                        //TODO: notify of error
                        ()
                    | None ->
                            let renderable = createView submitProposal operation.entityId operation.entity
                            renderTable <- Map.add operation.entityId renderable renderTable
                create
            | Parent operation ->
                let parent () =
                    let parentView = Map.tryFind operation.parentId renderTable
                    let childView = Map.tryFind operation.entityId renderTable
                    match parentView, childView with
                    | Some parentView, Some childView ->
                            parent parentView childView
                    | _ ->
                        //TODO: notify of error
                        ()
                parent
            | Reorder operation ->
                (fun () -> ())
            | Update operation ->
                (fun () -> ())
            | Orphan operation ->
                (fun () -> ())
            | Delete operation ->
                (fun () -> ())
        render
    
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
        let renderer = createRenderer ()
        let renderDispatcher = (fun renderCommand ->
            (fun () -> renderCommand ())
            |> mauiApp.Dispatcher.Dispatch
            |> ignore)
        application.WithRenderer renderer (Some renderDispatcher)
        application.Run ()
    do rootPage.Loaded.Add(complete)
    
    member mauiApp.ReplaceRootContent(view) =
        rootPage.Content <- view
