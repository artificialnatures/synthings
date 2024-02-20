namespace synthings.sandbox.console

open synthings.transmission

module ConsoleRenderer =
    open Spectre.Console

    let waitIndicator (message : string) =
        (*
        let waitStatus = AnsiConsole.Status()
        waitStatus.Spinner <- Spinner.Known.Aesthetic //Favorites: Aesthetic, Arrow3, BouncingBall, Default, Flip, GrowVertical, Layer, Pong
        waitStatus.Start(message, fun _ -> ())
        *)
        AnsiConsole.WriteLine message
    
    let menu () =
        let selectionPrompt = new SelectionPrompt<string>()
        selectionPrompt.Title <- "Menu"
        selectionPrompt.PageSize <- 20
        selectionPrompt.MoreChoicesText <- "[grey](Move up and down to reveal more choices)[/]"
        selectionPrompt.HighlightStyle <- Style.Parse("yellow")
        selectionPrompt.AddChoices("Enter Text") |> ignore
        let selection = AnsiConsole.Prompt(selectionPrompt)
        match selection with
        | "Enter Text" -> 
            let inputPrompt = new TextPrompt<string>("Enter Text: ")
            let inputText = AnsiConsole.Ask<string>("Enter Text: ")
            waitIndicator "Processing..."
            //Text(inputText) |> Replace |> sendMessage
        | _ -> ()
    (*
    let filePicker (submitProposal : ProposalDispatcher) =
        let inputText = Spectre.Console.AnsiConsole.Ask<string>("File Path: ")
        [inputText] |> LoadFiles |> submitProposal
    *)
    let fileDisplay (file : File) =
        Spectre.Console.AnsiConsole.WriteLine $"File: Path = {file.filePath}, Size = {file.sizeInBytes} bytes"
    
    let text (text : string) =
        Spectre.Console.AnsiConsole.WriteLine text

    let button (submitProposal : MessageDispatcher<Proposal<StateRepresentation>>) (renderableId : Identifier) (label : string) (onClickProposal : Proposal<StateRepresentation>) =
        let confirmed = AnsiConsole.Confirm (label, true)
        if confirmed then
            submitProposal renderableId onClickProposal
        ()

module Console =
    let render (submitProposal : MessageDispatcher<Proposal<StateRepresentation>>) (renderableId : Identifier) (stateRepresentation : StateRepresentation) =
        match stateRepresentation with
        | ApplicationContainer -> 
            Some stateRepresentation
        | Cursor ->
            Some stateRepresentation
        | Window _ ->
            Some stateRepresentation
        | VerticalStack ->
            Some stateRepresentation
        | Canvas ->
            Some stateRepresentation
        | Transform _ ->
            Some stateRepresentation
        | Button(label, onClickProposal) ->
            ConsoleRenderer.button submitProposal renderableId label onClickProposal
            Some stateRepresentation
        | Text text -> 
            ConsoleRenderer.text text
            Some stateRepresentation
        | Image _ -> None
        | File file -> 
            ConsoleRenderer.fileDisplay file
            Some stateRepresentation
        | FilePicker filePaths -> 
            //ConsoleRenderer.filePicker submitProposal
            Some stateRepresentation
        | Wait -> 
            ConsoleRenderer.waitIndicator "Loading..."
            Some stateRepresentation
        
    let handleRenderTask (submitProposal : MessageDispatcher<Proposal<StateRepresentation>>) (renderTask : RenderTask<StateRepresentation, StateRepresentation>) =
        match renderTask with
        | CreateView task ->
            render submitProposal task.renderableId task.entity
        | UpdateView task ->
            render submitProposal task.renderableId task.entity
        | DeleteView _ -> None
        | ParentView _ -> None
        | ReorderView _ -> None
        | OrphanView _ -> None

    [<EntryPoint>]
    let program (arguments : string array) =
        let goodbyeState =
            Node (
                VerticalStack,
                [
                    Leaf (Text "Goodbye, world!")
                ]
            )
        let helloState =
            Node (
                VerticalStack,
                [
                    Leaf (Text "Hello, world!")
                    Leaf (Button ("OK", Replace {entityToReplace = Ancestor 1; replacement = goodbyeState}))
                ]
            )
        let configuration =
            {
                messagingImplementation = Channels
                rendererImplementation = handleRenderTask
            }
        let app = Application<StateRepresentation, StateRepresentation>(configuration)
        let initialProposal =
            Initialize {
                initialTree = helloState
            }
        app.Enqueue Identifier.empty initialProposal
        app.Step ()
        app.RunBlocking()
        0