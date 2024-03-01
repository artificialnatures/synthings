namespace synthings.ui.maui

open Microsoft.Maui.Controls
open Microsoft.Maui

type InfiniteCanvas() as this =
    inherit ScrollView()
    
    do this.Background <- SolidColorBrush.BlanchedAlmond
    do this.Orientation <- ScrollOrientation.Both
    let content = AbsoluteLayout()
    do content.WidthRequest <- 10000.0
    do content.HeightRequest <- 10000.0
    do content.Background <- SolidColorBrush.Cornsilk
    let dropRecognizer = DropGestureRecognizer()
    do dropRecognizer.DragOver.Add (fun _ -> content.Background <- SolidColorBrush.LightPink)
    let onDrop (args : DropEventArgs) =
        let data = args.Data
        content.Background <- SolidColorBrush.Cornsilk
    do dropRecognizer.Drop.Add onDrop
    do content.GestureRecognizers.Add dropRecognizer
    do this.Content <- content
    let mutable previousTranslation = (0.0, 0.0)
    let mutable previousScale = 1.0
    
    let onPan (args : PanUpdatedEventArgs) =
        match args.StatusType with
        | GestureStatus.Started ->
            previousTranslation <- (this.TranslationX, this.TranslationY)
        | GestureStatus.Running ->
            let x, y = previousTranslation
            this.TranslationX <- x + args.TotalX
            this.TranslationY <- y + args.TotalY
        | GestureStatus.Completed ->
            this.TranslationX <- this.TranslationX + args.TotalX
            this.TranslationY <- this.TranslationY + args.TotalY
        | GestureStatus.Canceled ->
            let x, y = previousTranslation
            this.TranslationX <- x
            this.TranslationY <- y
        | _ -> ()
    
    let onZoom (args : PinchGestureUpdatedEventArgs) =
        match args.Status with
        | GestureStatus.Started ->
            previousScale <- content.Scale
        | GestureStatus.Running ->
            content.Scale <- content.Scale + (args.Scale - 1.0)
        | GestureStatus.Completed ->
            previousScale <- content.Scale
        | GestureStatus.Canceled ->
            content.Scale <- previousScale
        | _ -> ()
    
    let onRightClick (args : TappedEventArgs) =
        let importImage () =
            let location = args.GetPosition(content)
            if location.HasValue then
                let result =
                    Microsoft.Maui.Storage.FilePicker.Default.PickAsync()
                    |> Async.AwaitTask
                    |> Async.RunSynchronously
                if result <> null then
                    //result.ContentType
                    //result.FileName
                    //result.FullPath
                    let image = Image(Source=StreamImageSource(Stream=(fun _ -> result.OpenReadAsync())))
                    image.TranslationX <- location.Value.X
                    image.TranslationY <- location.Value.Y
                    content.Children.Add image
        this.Dispatcher.Dispatch(importImage)
        |> ignore

    let setupGestureRecognizers () =
        (*
        let pointerRecognizer = PointerGestureRecognizer()
        pointerRecognizer.PointerMoved.Add (fun args -> ())
        this.GestureRecognizers.Add pointerRecognizer
        *)
        let pinchRecognizer = PinchGestureRecognizer()
        pinchRecognizer.PinchUpdated.Add onZoom
        this.GestureRecognizers.Add pinchRecognizer
        (*
        let panRecognizer = PanGestureRecognizer()
        panRecognizer.PanUpdated.Add onPan
        this.GestureRecognizers.Add panRecognizer
        *)
        let tapRecognizer = TapGestureRecognizer(Buttons = ButtonsMask.Secondary, NumberOfTapsRequired = 1)
        tapRecognizer.Tapped.Add onRightClick
        this.GestureRecognizers.Add tapRecognizer

    do setupGestureRecognizers ()
    
    member this.AddChild (view : View) =
        let mutable isDragging = false
        let mutable originalLocation = (0.0, 0.0)
        let dragRecognizer = DragGestureRecognizer()
        dragRecognizer.DragStarting.Add (fun args ->
                let location = args.GetPosition(content)
                if location.HasValue then
                    originalLocation <- (location.Value.X, location.Value.Y)
                    isDragging <- true
            )
        dragRecognizer.DragStartingCommand <- Command(fun () ->
                isDragging <- true
            )
        view.GestureRecognizers.Add dragRecognizer
        let onDrop (args : DropEventArgs) =
            if isDragging then
                let location = args.GetPosition(content)
                if location.HasValue then
                    let originalX, originalY = originalLocation
                    let translationX = location.Value.X - originalX
                    let translationY = location.Value.Y - originalY
                    view.TranslationX <- view.TranslationX + translationX
                    view.TranslationY <- view.TranslationY + translationY
                    originalLocation <- (originalX + translationX, originalY + translationY)
                isDragging <- false
        dropRecognizer.Drop.Add onDrop
        content.Add view
