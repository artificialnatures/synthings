module synthings.core.tests.Views

open Xunit
open synthings.core
open synthings.core

[<Fact>]
let ``Application has views for menu and graph initially`` () =
    let application = Application()
    Assert.Equal(2, List.length application.Views)
    Assert.Equal("Behavior Menu", (List.item 0 application.Views).DisplayName)
    Assert.Equal("Graph", (List.item 1 application.Views).DisplayName)

[<Fact>]
let ``Adding a machine to the graph creates a new machine and a new view returned in a ChangeSet`` () =
    let application = Application()
    let behavior =
        application.Library.listTopics()
        |> List.head
        |> application.Library.listBehaviors
        |> List.find (fun (behavior : BehaviorDescriptor) -> behavior.DisplayName = "Relay")
    let changeSet = application.CreateMachine behavior
    Assert.Equal(1, List.length changeSet.MachineChanges)
    Assert.Equal(Create, changeSet.MachineChanges.Head.Operation)
    Assert.Equal(1, List.length changeSet.ViewChanges)
    Assert.Equal(Create, changeSet.ViewChanges.Head.Operation)
