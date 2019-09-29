module synthings.library.tests.Performance

open Xunit
open synthings.core

[<Fact>]
let ``Assess performance of simple graph`` () =
    let stopwatch = new System.Diagnostics.Stopwatch()
    stopwatch.Start()
    let library = aggregateLibrary.build()
    let topics = library.listTopics()
    let waveTopic = List.find (fun (topic : TopicDescriptor) -> topic.DisplayName.Contains "Wave") topics
    let waveBehaviors = library.listBehaviors waveTopic
    let sineBehavior = List.find (fun (behaviorDescriptor : BehaviorDescriptor) -> behaviorDescriptor.DisplayName.Contains "Sine") waveBehaviors
    let sineMachine = library.createMachine sineBehavior
    let testGraph =
        Graph.empty
        |> Graph.addMachine sineMachine
        |> Graph.connectToRoot sineMachine.Id
    let signals = Signal.createUniformSeries (time.now ()) 0.0 10000.0 1.0 0.0
    let inducer = Graph.induce testGraph
    Seq.iter inducer signals
    stopwatch.Stop()
    let elapsedTime = stopwatch.Elapsed.TotalSeconds
    Assert.True(elapsedTime < 1.0)

[<Fact>]
let ``Assess performance of simple graph with a monitor`` () =
    let stopwatch = new System.Diagnostics.Stopwatch()
    stopwatch.Start()
    let library = aggregateLibrary.build()
    let topics = library.listTopics()
    let waveTopic = List.find (fun (topic : TopicDescriptor) -> topic.DisplayName.Contains "Wave") topics
    let waveBehaviors = library.listBehaviors waveTopic
    let sineBehavior = List.find (fun (behaviorDescriptor : BehaviorDescriptor) -> behaviorDescriptor.DisplayName.Contains "Sine") waveBehaviors
    let sineMachine = library.createMachine sineBehavior
    let monitor = Monitor(FrameLimit(10))
    let testGraph =
        Graph.empty
        |> Graph.addMachine sineMachine
        |> Graph.addMachine monitor.Machine
        |> Graph.connectToRoot sineMachine.Id
        |> Graph.connect sineMachine.Id monitor.Machine.Id
    let signals = Signal.createUniformSeries (time.now ()) 0.0 10000.0 1.0 0.0
    let inducer = Graph.induce testGraph
    Seq.iter inducer signals
    stopwatch.Stop()
    let elapsedTime = stopwatch.Elapsed.TotalSeconds
    Assert.True(elapsedTime < 1.0)
