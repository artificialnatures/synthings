namespace synthings.scalar
open synthings.core

type ScalarTopic =
    | Wave
    | Envelope
    interface TopicIdentifier

module library =
    let internal topics () =
        [
            {Topic = Wave; DisplayName = "Wave"; Id = Identifier.fromString("68C8D99F-1452-408E-9D2C-EC7F07DB7F93")};
            {Topic = Envelope; DisplayName = "Envelope"; Id = Identifier.fromString("09D5E3CF-03EC-4E19-8EC0-62B75A408C0D")}
        ]
    
    let internal listBehaviors (topicIdentifier : TopicIdentifier) =
        match topicIdentifier with
        | :? ScalarTopic as scalarTopic ->
            match scalarTopic with
            | Wave -> wave.behaviorDescriptors
            | Envelope -> envelope.behaviorDescriptors
        | _ -> List.empty
    
    let internal behaviorWithIdentifier (behaviorIdentifier : BehaviorIdentifier) =
        match behaviorIdentifier with
        | :? WaveBehavior as waveBehavior -> wave.findBehaviorDescriptor behaviorIdentifier
        | :? EnvelopeBehavior as envelopeBehavior -> envelope.findBehaviorDescriptor behaviorIdentifier
        | _ -> failwith "Could not find BehaviorIdentifier."
    
    let internal createMachine (behaviorIdentifier : BehaviorIdentifier) =
        match behaviorIdentifier with
        | :? WaveBehavior as waveBehavior -> wave.createMachine waveBehavior WaveParameters.Default
        | :? EnvelopeBehavior as envelopeBehavior -> envelope.createMachine envelopeBehavior EnvelopeParameters.Default
        | _ -> Machine.createError()

type ScalarLibrary() =
    static member build () = ScalarLibrary() :> LibraryResolver
    member this.Origin = (this :> LibraryResolver).Origin
    member this.Name = (this :> LibraryResolver).Name
    member this.listTopics () = (this :> LibraryResolver).listTopics ()
    member this.listBehaviors topicDescriptor = (this :> LibraryResolver).listBehaviors topicDescriptor
    member this.createMachine behaviorDescriptor = (this :> LibraryResolver).createMachine behaviorDescriptor
    member this.behaviorWithIdentifier (behaviorIdentifier : BehaviorIdentifier) = library.behaviorWithIdentifier behaviorIdentifier
    interface LibraryResolver with
        member this.Origin = typeof<ScalarTopic>.Namespace
        member this.Name = "Scalar"
        member this.listTopics () = library.topics ()
        member this.listBehaviors topicDescriptor = library.listBehaviors topicDescriptor.Topic
        member this.createMachine behaviorDescriptor = library.createMachine behaviorDescriptor.Behavior
