namespace synthings.scalar
open synthings.core

type ScalarTopic =
    | Wave
    | Envelope
    interface TopicIdentifier

module library =
    open System
    let topics () =
        [
            {Topic = Wave; DisplayName = "Wave"; Id = Guid.Parse("68C8D99F-1452-408E-9D2C-EC7F07DB7F93")};
            {Topic = Envelope; DisplayName = "Envelope"; Id = Guid.Parse("09D5E3CF-03EC-4E19-8EC0-62B75A408C0D")}
        ]
    
    let listBehaviors (topicIdentifier : TopicIdentifier) =
        match topicIdentifier with
        | :? ScalarTopic as scalarTopic ->
            match scalarTopic with
            | Wave -> wave.listBehaviors ()
            | Envelope -> envelope.listBehaviors ()
        | _ -> List.empty
    
    let createMachine (behaviorIdentifier : BehaviorIdentifier) =
        match behaviorIdentifier with
        | :? WaveBehavior as waveBehavior -> wave.createMachine waveBehavior WaveParameters.Default
        | :? EnvelopeBehavior as envelopeBehavior -> envelope.createMachine envelopeBehavior EnvelopeParameters.Default
        | _ -> Machine.createError()

type ScalarLibrary() =
    member this.createMachine (behaviorIdentifier : BehaviorIdentifier) = library.createMachine behaviorIdentifier
    interface LibraryResolver with
        member this.Origin = typeof<ScalarTopic>.Namespace
        member this.Name = "Scalar"
        member this.listTopics () = library.topics ()
        member this.listBehaviors topicDescriptor = library.listBehaviors topicDescriptor.Topic
        member this.createMachine behaviorDescriptor = library.createMachine behaviorDescriptor.Behavior
