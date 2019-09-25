module synthings.library.tests.Libraries

open Xunit
open synthings.core

[<Fact>]
let ``Find a topic in the aggregate library`` () =
    //assumes a reference to synthings.scalar
    let library = aggregateLibrary.build()
    let topics = library.listTopics()
    let waveTopics = List.filter (fun (topic : TopicDescriptor) -> topic.DisplayName.Contains "Wave") topics
    Assert.True(waveTopics.Length = 1)
    
[<Fact>]
let ``Create a machine from a behavior in the aggregate library`` () =
    //assumes a reference to synthings.scalar
    let library = aggregateLibrary.build()
    let topics = library.listTopics()
    let waveTopic = List.find (fun (topic : TopicDescriptor) -> topic.DisplayName.Contains "Wave") topics
    let waveBehaviors = library.listBehaviors waveTopic
    let sineBehavior = List.find (fun (behaviorDescriptor : BehaviorDescriptor) -> behaviorDescriptor.DisplayName.Contains "Sine") waveBehaviors
    let sineMachine = library.createMachine sineBehavior
    Assert.True(sineMachine.Name.Contains "Sine")
