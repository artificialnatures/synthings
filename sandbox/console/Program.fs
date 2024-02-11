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

    let button (submitProposal : MessageDispatcher<Proposal<StateRepresentation>>) (label : string) (onClickProposal : Proposal<StateRepresentation>) =
        let confirmed = AnsiConsole.Confirm (label, true)
        if confirmed then
            submitProposal onClickProposal
        else
            ()

module Console =
    let render (submitProposal : MessageDispatcher<Proposal<StateRepresentation>>) (renderableId : Identifier) (stateRepresentation : StateRepresentation) =
        match stateRepresentation with
        | ApplicationContainer _ -> ()
        | Cursor -> ()
        | Window _ -> ()
        | VerticalStack -> ()
        | Canvas -> ()
        | Transform _ -> ()
        | Button(label, onClickProposal) ->
            ConsoleRenderer.button submitProposal label onClickProposal
        | Text text -> 
            ConsoleRenderer.text text
        | Image _ -> ()
        | File file -> 
            ConsoleRenderer.fileDisplay file
        | FilePicker filePaths -> 
            //ConsoleRenderer.filePicker submitProposal
            ()
        | Wait -> 
            ConsoleRenderer.waitIndicator "Loading..."
        
    let handleRenderTask (submitProposal : MessageDispatcher<Proposal<StateRepresentation>>) (renderTask : RenderTask<StateRepresentation, Identifier>) =
        match renderTask with
        | CreateView createTask ->
            render submitProposal createTask.renderableId createTask.entity
            createTask.renderableId
        | UpdateView updateTask ->
            updateTask.renderableId
        | DeleteView deleteTask ->
            deleteTask.renderableId

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
                    Leaf (Button ("OK", Replace {entityToReplace = Parent; replacement = goodbyeState}))
                ]
            )
        let configuration =
            {
                messagingImplementation = Channels
                rootEntity = ApplicationContainer
                rootRenderable = Identifier.create()
                renderer = handleRenderTask
                initialProposal = Add {parent = Identifier.create(); siblingToPrecede = None; entityToAdd = helloState}
            }
        Application<StateRepresentation, Identifier>(configuration).RunBlocking()
        0