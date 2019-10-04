namespace synthings.core
open aggregateLibrary

type Application() =
    let mutable _library = AggregateLibrary()
    let mutable _graph = Graph.empty
    let initialViews =
        [
            View.create (Identifier.create()) "Behavior Menu";
            View.create (Identifier.create()) "Graph";
            View.create _graph.Root.Id _graph.Root.Name
        ]
    let mutable _viewMap = List.map (fun (view : View) -> (view.SubjectId, view)) initialViews |> Map.ofList
    let mutable _changeSet = ChangeSet.empty
    member this.Library = _library :> LibraryResolver
    member this.Graph = _graph
    member this.Views = Map.toList _viewMap |> List.map (fun (key, value) -> value)
    member this.AddView (view : View) =
        _viewMap <- Map.add view.SubjectId view _viewMap
    member this.CreateMachine (behaviorDescriptor : BehaviorDescriptor) =
        let machine = _library.createMachine behaviorDescriptor
        _graph <- Graph.addMachine machine _graph
        let view = View.forMachine machine
        this.AddView view
        ChangeSet.machineCreated machine view
    member this.Connect (upstreamId : Identifier) (downstreamId : Identifier) =
        _graph <- Graph.connect upstreamId downstreamId _graph
        let connection = ConnectionSet.findConnection upstreamId downstreamId _graph.Connections
        let view = View.forConnection connection
        this.AddView view
        ChangeSet.connectionCreated connection view
    member this.ConnectToRoot (downstreamId : Identifier) =
        this.Connect _graph.Root.Id downstreamId
    member this.Record (upstreamId : Identifier) (message : Message) =
        if Map.containsKey upstreamId _viewMap then
            let view = _viewMap.Item upstreamId
            let revisedView = {view with History = Seq.append view.History [message.Signal]}
            this.AddView revisedView
            _changeSet <- {_changeSet with ViewChanges = List.append _changeSet.ViewChanges [Change.update revisedView]}
    member this.Forward (upstreamId : Identifier) (message : Message) =
        this.Record upstreamId message
        Graph.sendFrom _graph upstreamId message
    member this.Induce (signal : Signal) =
        _changeSet <- ChangeSet.empty
        Graph.induce this.Forward _graph signal
        _changeSet
