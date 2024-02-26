namespace synthings.transmission

type Renderer<'entity> = MessageDispatcher<Proposal<'entity>> -> Operation<'entity> -> unit

type Application<'entity>() =
    let mutable entityTable : EntityTable<'entity> = EntityTable.empty
    let mutable renderChangeSet : Renderer<'entity> = (fun _ _ -> ())
    let mutable history : History<'entity> option = None
    let submitProposal, receiveProposal =
        MessageQueue.create<Proposal<'entity>, ChangeSet<'entity>> Channels
    let accept message =
        ChangeSet.assemble entityTable message
    let react changeSet =
        match changeSet with
        | Ok changeSet ->
            entityTable <- EntityTable.apply entityTable changeSet
            Ok changeSet
        | Error error ->
            Error error
    let record changeSet =
        match history with
        | Some historyList ->
            match changeSet with
            | Ok changeSet ->
                history <- List.append [ changeSet ] historyList |> Some
                Ok changeSet
            | Error error ->
                Error error
        | None -> changeSet
    let render changeSet =
        match changeSet with
        | Ok changeSet ->
            List.iter (renderChangeSet submitProposal) changeSet
        | Error _ -> ()
    let processMessage message =
        accept message
        |> react
        |> record
        |> render
    let mutable isRunning = false
    let applicationRunner =
        async {
            while isRunning do
                match receiveProposal() with
                | Some message ->
                    processMessage message
                | None -> ()
        }
    /// <summary>Provide a render function to the Application. The render function will be called for each Operation in each ChangeSet.</summary>
    member app.WithRenderer (render : Renderer<'entity>) =
        renderChangeSet <- render
    /// <summary>Manually enqueue a Message. Call Step after Enqueue to process the message. For testing or blocking execution environments, e.g. console apps.</summary>
    member app.Enqueue entityId proposal =
        submitProposal entityId proposal
    /// <summary>Manually advance the Application one step, processing the next message in the queue. For testing or blocking execution environments, e.g. console apps.</summary>
    member app.Step () =
        let rec retrieveNextMessage () =
            match receiveProposal() with
            | Some message ->
                processMessage message
            | None ->
                retrieveNextMessage ()
        retrieveNextMessage ()
    /// <summary>Call a function for each entity in the application state. For testing purposes.</summary>
    member app.ForEachEntity (action : (Proposal<'entity> -> unit) -> 'entity -> unit) =
        let entities = entityTable.entities |> Map.toList
        List.iter (fun (identifier, entity) -> action (submitProposal identifier) entity) entities
    /// <summary>Get a snapshot of the Application state. Useful for testing and debugging.</summary>
    /// <returns>A Tree representing the state of the application.</returns>
    member app.BuildTree () =
        EntityTable.buildTree None entityTable
    ///<summary>Run the Application without blocking the main thread. The typical run scenario for GUI apps.</summary>
    member app.Run () =
        isRunning <- true
        Async.Start applicationRunner
    /// <summary>Run the Application and block the main thread. Call Application.Step to manually advance to the next state. The typical run scenario for tests and console apps.</summary>
    member app.RunBlocking () =
        isRunning <- true
        applicationRunner |> Async.RunSynchronously
    /// <summary>Shut down message processing, event loops, and exit the process.</summary>
    member app.Quit () =
        // TODO: Ensure that any messaging queues (sockets, channels, etc.) are disposed
        isRunning <- false
