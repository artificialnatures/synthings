module synthings.scalar.tests.Waves

open Xunit
open synthings.core
open synthings.core
open synthings.core
open synthings.scalar

[<Fact>]
let ``Sine wave produces oscillating values`` () =
    //TODO: refactor using Application
    (*
    let scalarLibrary = ScalarLibrary()
    let sineWaveMachine = scalarLibrary.createMachine WaveBehavior.SineWave
    let expected = [0.0; 1.0; 0.0; -1.0; 0.0; 1.0; 0.0; -1.0; 0.0]
    let mutable actual = List.empty
    let record (signal : Signal) =
        actual <- List.append actual [signal.Value]
        signal
    let monitoredMachine = Machine.createMonitor sineWaveMachine record
    let messages =
        Signal.createUniformSeries (time.now ()) 0.0 2.0 0.25 0.0
        |> Seq.map Message.packWithoutForwarding
    Seq.iter monitoredMachine.Input messages
    Assert.True(number.listsAreIdentical expected actual)
    *)
    let application = Application()
    let behaviorDescriptor =
        application.Library.listTopics ()
        |> List.find (fun topicDescriptor -> topicDescriptor.DisplayName.Contains "Wave")
        |> application.Library.listBehaviors
        |> List.find (fun behaviorDescriptor -> behaviorDescriptor.DisplayName.Contains "Sine")
    let machineCreated = application.CreateMachine behaviorDescriptor
    application.ConnectToRoot machineCreated.MachineChanges.Head.Subject.Id
    Assert.True(false)
