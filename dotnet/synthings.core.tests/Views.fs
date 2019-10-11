module synthings.core.tests.Views

open Xunit
open synthings.core

[<Fact>]
let ``Application has views for menu, graph and root initially`` () =
    let application = Application()
    Assert.Equal(3, List.length application.Views)
    let behaviorMenuFound = List.exists (fun (view : View) -> view.DisplayName = "Behavior Menu") application.Views
    Assert.True(behaviorMenuFound)
    let graphFound = List.exists (fun (view : View) -> view.DisplayName = "Graph") application.Views
    Assert.True(graphFound)
    let rootFound = List.exists (fun (view : View) -> view.DisplayName = "Root") application.Views
    Assert.True(rootFound)

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

[<Fact>]
let ``Connecting machines in the graph creates a view representing the connection`` () =
    let application = Application()
    let behavior =
        application.Library.listTopics()
        |> List.head
        |> application.Library.listBehaviors
        |> List.find (fun (behavior : BehaviorDescriptor) -> behavior.DisplayName = "Relay")
    let machineCreated = application.CreateMachine behavior
    let connectionCreated = application.ConnectToRoot machineCreated.MachineChanges.Head.Subject.Id
    Assert.Equal(1, connectionCreated.ViewChanges.Length)

[<Fact>]
let ``A machine view records the signals that pass through the machine`` () =
    let application = Application()
    let behavior =
        application.Library.listTopics()
        |> List.head
        |> application.Library.listBehaviors
        |> List.find (fun (behavior : BehaviorDescriptor) -> behavior.DisplayName = "Relay")
    let machineCreated = application.CreateMachine behavior
    let relayMachine = machineCreated.MachineChanges.Head.Subject
    application.ConnectToRoot machineCreated.MachineChanges.Head.Subject.Id |> ignore //don't need a reference to the connection
    let values = [0.0; 1.0; 2.0; 3.0; 4.0; 5.0]
    let epoch = Instant.now()
    let signals = Signal.createSeries epoch 0.0 1.0 values
    let mutable changeSet = ChangeSet.empty
    let induce signal = changeSet <- application.Induce signal
    List.iter induce signals
    let change = List.find (fun (change : Change<View>) -> change.Subject.SubjectId = relayMachine.Id) changeSet.ViewChanges
    let machineView = change.Subject
    Assert.Equal(values.Length, machineView.Recording.Signals.Length)
