module synthings.core.tests.Libraries

open Xunit
open synthings.core

[<Fact>]
let ``Create library`` () =
    let coreLibrary = library.create ()
    let firstTopic = coreLibrary.Libraries.Head.Topics.Head
    Assert.Equal(1, coreLibrary.Libraries.Head.Topics.Length)
    Assert.Equal("Basic", firstTopic.Identifier.Name)
    Assert.Equal(2, firstTopic.Behaviors.Length)

[<Fact>]
let ``Create a machine from a library`` () =
    let coreLibrary = library.create ()
    let firstTopic = coreLibrary.Libraries.Head.Topics.Head
    let firstBehavior = firstTopic.Behaviors.Head
    let machine = firstTopic.BuildMachine firstBehavior.Id
    Assert.Equal("Relay", machine.Name)
