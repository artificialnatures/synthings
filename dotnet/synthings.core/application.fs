namespace synthings.core
open aggregateLibrary

module application =
    let internal buildBehaviorMenuView (library : AggregateLibrary) =
        {Id = identifier.create(); DisplayName = "Behavior Menu"}
    let internal buildGraphView (graph : Graph) =
        {Id = identifier.create(); DisplayName = "Graph"}
    
    let internal buildViews (library : AggregateLibrary) (graph : Graph) =
        let behaviorMenuView = buildBehaviorMenuView library
        let graphView = buildGraphView graph
        [behaviorMenuView; graphView]

type Application() =
    let mutable _library = AggregateLibrary()
    let mutable _graph = Graph.empty
    let mutable _views = application.buildViews _library _graph
    member this.Library = _library :> LibraryResolver
    member this.Graph = _graph
    member this.Views = _views
    member this.CreateMachine (behaviorDescriptor : BehaviorDescriptor) =
        let machine = _library.createMachine behaviorDescriptor
        _graph <- Graph.addMachine machine _graph
        let view = View.forMachine machine
        _views <- List.append _views [view]
        ChangeSet.machineCreated machine view
