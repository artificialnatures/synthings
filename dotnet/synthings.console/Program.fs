open System
open System.Threading
open synthings.core

[<EntryPoint>]
let main argv =
    let library = aggregateLibrary.build()
    let topics = library.listTopics()
    let waveTopic = List.find (fun (topic : TopicDescriptor) -> topic.DisplayName.Contains "Wave") topics
    let waveBehaviors = library.listBehaviors waveTopic
    let sineBehavior = List.find (fun (behaviorDescriptor : BehaviorDescriptor) -> behaviorDescriptor.DisplayName.Contains "Sine") waveBehaviors
    let sineMachine = library.createMachine sineBehavior
    let monitor = Monitor(FrameLimit(10))
    let testGraph =
        graph.empty
        |> graph.addMachine sineMachine
        |> graph.addMachine monitor.Machine
        |> graph.connectToRoot sineMachine.Id
        |> graph.connect sineMachine.Id monitor.Machine.Id
    let inducer = graph.induce testGraph
    let epoch = time.now()
    let mutable frameCount = 0
    while frameCount < 100000 do
        inducer (Signal.createNow epoch 0.0)
        let totalTime = (monitor.LatestSignal.Time - epoch).TotalSeconds
        printfn "%i value = %f at %f" frameCount monitor.LatestValue totalTime
        Thread.Sleep 20
        frameCount <- frameCount + 1
    printf "Done."
    0
