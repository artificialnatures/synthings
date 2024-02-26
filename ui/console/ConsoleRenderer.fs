namespace synthings.ui.console

open synthings.transmission
open Spectre.Console

module ConsoleRenderer =
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
            AnsiConsole.WriteLine inputText
        | _ -> ()
    
    let display (submitProposal : MessageDispatcher<Proposal<StateRepresentation>>) (renderableId : Identifier) (stateRepresentation : StateRepresentation) =
        match stateRepresentation with
        | ApplicationContainer -> 
            ()
        | Cursor ->
            ()
        | Window _ ->
            ()
        | VerticalStack ->
            ()
        | Canvas ->
            ()
        | Transform _ ->
            ()
        | Button(label, onClickProposal) ->
            let confirmed = AnsiConsole.Confirm (label, true)
            if confirmed then
                submitProposal renderableId onClickProposal
            ()
        | Text text -> 
            AnsiConsole.WriteLine text
        | Image _ ->
            ()
        | File file -> 
            AnsiConsole.WriteLine $"File: Path = {file.filePath}, Size = {file.sizeInBytes} bytes"
        | FilePicker filePaths -> 
            ()
        | Wait -> 
            let waitStatus = AnsiConsole.Status()
            waitStatus.Spinner <- Spinner.Known.Aesthetic //Favorites: Aesthetic, Arrow3, BouncingBall, Default, Flip, GrowVertical, Layer, Pong
            waitStatus.Start("Processing", fun _ -> ())
    
    let render submitProposal operation =
        match operation with
        | Create operation ->
            display submitProposal operation.entityId operation.entity
        | Parent operation -> ()
        | Reorder operation -> ()
        | Update operation -> ()
        | Orphan operation -> ()
        | Delete operation -> ()
