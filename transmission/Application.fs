namespace synthings.transmission

type ApplicationConfiguration<'entity, 'renderable> =
    {
        messagingImplementation : MessagingImplementation
        renderer : Renderer<'entity, 'renderable>
    }

type Application<'entity, 'renderable>(configuration : ApplicationConfiguration<'entity, 'renderable>) =
    let mutable entityTable : EntityTable<'entity> = EntityTable.empty 
    let mutable history : History<'entity> = List.empty
    let renderer : Renderer<'entity, 'renderable> = configuration.renderer
    let mutable renderTable : RenderTable<'renderable> = Map.empty
    let submitProposal, receiveProposal =
        MessageQueue.create<Proposal<'entity>, ChangeSet<'entity>> configuration.messagingImplementation

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
        match changeSet with
        | Ok changeSet ->
            history <- List.append [ changeSet ] history
            Ok changeSet
        | Error error ->
            Error error
    
    let render changeSet =
        match changeSet with
        | Ok changeSet ->
            renderTable <- Renderer.renderChangeSet submitProposal renderer renderTable changeSet
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

    /// <summary>Manually enqueue a Message. Call Step after Enqueue to process the message. For testing or blocking execution environments, e.g. console apps.</summary>
    member app.Enqueue entityId proposal =
        submitProposal entityId proposal
    
    /// <summary>Manually advance the Application one step, processing the next message in the queue. For testing or blocking execution environments, e.g. console apps.</summary>
    member app.Step () =
        let rec retreiveNextMessage () =
            match receiveProposal() with
            | Some message ->
                processMessage message
            | None ->
                retreiveNextMessage ()
        retreiveNextMessage ()
    
    member app.FindRenderable renderableId =
        Map.tryFind renderableId renderTable

    /// <summary>Get a snapshot of the Application state. Useful for testing and debugging.</summary>
    /// <returns>A Tree<'entity> representing the state of the application.</returns>
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
