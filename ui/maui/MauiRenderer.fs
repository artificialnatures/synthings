namespace synthings.ui.maui

open synthings.transmission

type MauiView = Microsoft.Maui.Controls.View
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
    
    let render findRenderable submitProposal renderTask =
        match renderTask with
        | CreateView task -> 
            let renderable = createView submitProposal task.renderableId task.entity
            Some renderable
        | ParentView task ->
            let parentView = findRenderable task.parentRenderableId
            let childView = findRenderable task.childRenderableId
            match parentView, childView with
            | Some parentView, Some childView ->
                parent parentView childView
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
