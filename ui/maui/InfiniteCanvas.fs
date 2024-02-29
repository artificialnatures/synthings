namespace synthings.ui.maui

open Microsoft.Maui.Controls
open Microsoft.Maui

open synthings.transmission

type InfiniteCanvas() as this =
    inherit ScrollView()
    
    let content = AbsoluteLayout()
    do content.Background <- SolidColorBrush.BlanchedAlmond
    let dropRecognizer = DropGestureRecognizer()
    do dropRecognizer.DragOver.Add (fun _ -> content.Background <- SolidColorBrush.LightPink)
    do dropRecognizer.Drop.Add (fun _ -> content.Background <- SolidColorBrush.BlanchedAlmond)
    do content.GestureRecognizers.Add dropRecognizer
    do this.Content <- content
    do this.Orientation <- ScrollOrientation.Both
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
        let position =
            let mousePosition = args.GetPosition(this)
            if mousePosition.HasValue
            then (mousePosition.Value.X, mousePosition.Value.Y)
            else (0.0, 0.0)
        //AddChild (Transform (position, Image DotNetBot), canvas)
        //|> submitProposal
        ()

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
        (*
        let mutable previousLocation : (float * float) option = None
        let mutable location : (float * float) option = None
        *)
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
        (*
        dragRecognizer.DropCompletedCommand <- Command(fun () ->
                match location with
                | Some (x, y) ->
                    view.AnchorX <- x
                    view.AnchorY <- y
                | None -> ()
            )
        *)
        view.GestureRecognizers.Add dragRecognizer
        (*
        let pointerRecognizer = PointerGestureRecognizer()
        let onMoved (args : PointerEventArgs) =
            match location with
            | Some _ ->
                let point = args.GetPosition(this)
                if point.HasValue
                then location <- Some (point.Value.X, point.Value.Y)
            | None -> ()
        pointerRecognizer.PointerMoved.Add onMoved
        view.GestureRecognizers.Add pointerRecognizer
        *)
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
