module synthings.core.tests.Applications

open Xunit
open synthings.core

[<Fact>]
let ``Creating a graph using Application`` () =
    let application = Application()
    //Default graph has a single machine:
    Assert.Equal(1, Map.count application.Graph.Machines)
    let firstBehaviorInLibrary =
        application.Library.listTopics()
        |> List.head
        |> application.Library.listBehaviors
        |> List.head
    application.createMachine firstBehaviorInLibrary
    Assert.Equal(2, Map.count application.Graph.Machines)
