namespace synthings.core
open aggregateLibrary

type Application() =
    let mutable library = AggregateLibrary()
    let mutable graph = Graph.empty
    member this.Library = library :> LibraryResolver
    member this.Graph = graph
    member this.createMachine (behaviorDescriptor : BehaviorDescriptor) =
        let machine = library.createMachine behaviorDescriptor
        graph <- Graph.addMachine machine graph
