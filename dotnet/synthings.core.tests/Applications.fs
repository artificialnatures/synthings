module synthings.core.tests.Applications

open Xunit
open synthings.core

[<Fact>]
let ``Creating a graph using Application`` () =
    let application = Application()
    Assert.Equal(1, Map.count application.Graph.Machines) //Default graph has a single machine
    let firstBehaviorInLibrary =
        application.Library.listTopics()
        |> List.head
        |> application.Library.listBehaviors
        |> List.find (fun behavior -> behavior.DisplayName.Contains "Relay")
    let machineCreated = application.CreateMachine firstBehaviorInLibrary
    Assert.Equal(1, machineCreated.MachineChanges.Length) //a new machine is added to the graph
    Assert.Equal(1, machineCreated.ViewChanges.Length) //a new view is created to represent the machine
    Assert.Equal(0, machineCreated.ConnectionChanges.Length) //no connections were created
    Assert.Equal(2, Map.count application.Graph.Machines) //now there are 2 machines
    let relayMachine = machineCreated.MachineChanges.Head.Subject
    let connectionCreated = application.ConnectToRoot relayMachine.Id
    Assert.Equal(1, connectionCreated.ConnectionChanges.Length) //a connection was created
    Assert.Equal(1, connectionCreated.ViewChanges.Length) //a view was created to represent the connection
    let values = [0.0; 1.0; 2.0; 3.0; 4.0; 5.0]
    let epoch = Instant.now()
    let signals = Signal.createSeries epoch 0.0 1.0 values
    //Each signal induced causes changes to the views representing the machines in the graph
    let mutable changeSet = ChangeSet.empty
    let induce signal = changeSet <- application.Induce signal
    List.iter induce signals
    //Find the view for relayMachine in the ChangeSet
    let change = List.find (fun (change : Change<View>) -> change.Subject.SubjectId = relayMachine.Id) changeSet.ViewChanges
    Assert.Equal(Operation.Update, change.Operation)
    Assert.Equal(relayMachine.Name, change.Subject.DisplayName)
    let history = List.map Signal.unpack change.Subject.Recording.Signals
    Assert.True(number.listsAreIdentical values history)
