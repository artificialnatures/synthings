namespace synthings.ui.maui

open Microsoft.Maui.Controls
open Microsoft.Maui

open synthings.transmission

type InfiniteCanvas() as this =
    inherit ScrollView()
    
    let content = AbsoluteLayout()
    do content.Background <- SolidColorBrush.Fuchsia
    do content.AnchorX <- 0.0
    do content.AnchorY <- 0.0
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
        content.Add view
