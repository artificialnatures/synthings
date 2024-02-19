namespace synthings.ui.maui

open Microsoft.Maui.Controls
open Microsoft.Maui

open synthings.transmission

type InfiniteCanvas(canvas : StateRepresentation, submitProposal : MessageDispatcher<Proposal<StateRepresentation>>) as this =
    inherit AbsoluteLayout()

    let mutable previousTranslation = (0.0, 0.0)

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
        | GestureStatus.Canceled -> ()
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
        let pointerRecognizer = PointerGestureRecognizer()
        pointerRecognizer.PointerMoved.Add (fun args -> ())
        this.GestureRecognizers.Add pointerRecognizer
        let pinchRecognizer = PinchGestureRecognizer()
        pinchRecognizer.PinchUpdated.Add (fun args -> ())
        this.GestureRecognizers.Add pinchRecognizer
        let panRecognizer = PanGestureRecognizer()
        panRecognizer.PanUpdated.Add onPan
        this.GestureRecognizers.Add panRecognizer
        let tapRecognizer = TapGestureRecognizer(Buttons = ButtonsMask.Secondary, NumberOfTapsRequired = 1)
        tapRecognizer.Tapped.Add onRightClick
        this.GestureRecognizers.Add tapRecognizer

    do setupGestureRecognizers ()
