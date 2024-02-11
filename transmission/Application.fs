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

    let accept proposal =
        ChangeSet.assemble entityTable proposal

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

    let step proposal =
        accept proposal
        |> react
        |> record
        |> render
    
    let mutable isRunning = false

    let applicationRunner =
        async {
            while isRunning do
                match receiveProposal() with
                | Some proposal ->
                    step proposal
                | None -> ()
        }

    member app.Step proposal =
        step proposal
    
    member app.buildTree () =
        EntityTable.buildTree None entityTable
    
    member app.buildDetailedTree () =
        EntityTable.buildTree None entityTable
    
    member app.Run () =
        isRunning <- true
        Async.Start applicationRunner

    member app.RunBlocking () =
        isRunning <- true
        applicationRunner |> Async.RunSynchronously
    
    member app.Quit () =
        isRunning <- false
