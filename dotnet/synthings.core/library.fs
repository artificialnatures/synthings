namespace synthings.core

type CoreTopic =
    | Core
    interface TopicIdentifier

type CoreBehavior =
    | Relay
    | Error
    interface BehaviorIdentifier

type CoreParameters =
    | Default
    interface Parameters

module library =
    open System
    let relay (signal : Signal) = signal
    let error (signal : Signal) = {signal with Value = 0.0} //TODO: add error handling
    
    let topics () =
        [
            {Topic = Core; DisplayName = "Core"; Id = Guid.Parse("56DBAC92-F1A6-400C-8640-3C48CD10A2CC")}
        ]
    
    let behaviors =
        [
            {Behavior = Relay; DisplayName = "Relay"; Id = Guid.Parse("F6D899DB-FF57-475D-94A1-2A1E0FB64DB4")};
            {Behavior = Error; DisplayName = "Error"; Id = Guid.Parse("2ACDA383-51D6-4946-8554-635ED1749C84")}
        ]
    
    let listBehaviors (topicIdentifier : TopicIdentifier) =
        match topicIdentifier with
        | :? CoreTopic as coreTopic ->
            match coreTopic with
            | Core -> behaviors
        | _ -> List.empty
    
    let createMachine (behaviorIdentifier : BehaviorIdentifier) =
        match behaviorIdentifier with
        | :? CoreBehavior as coreBehavior ->
            match coreBehavior with
            | Relay -> machine.createMachine "Relay" relay
            | Error -> machine.createMachine "Error" error
        | _ -> machine.createMachine "Error" error
    
    type internal CoreLibrary() =
        member this.listTopics () = (this :> LibraryResolver).listTopics ()
        member this.listBehaviors topicIdentifier = (this :> LibraryResolver).listBehaviors topicIdentifier
        member this.createMachine behaviorIdentifier = (this :> LibraryResolver).createMachine behaviorIdentifier
        interface LibraryResolver with
            member this.Origin = typeof<CoreTopic>.Namespace
            member this.Name = "Core"
            member this.listTopics () = topics()
            member this.listBehaviors topicDescriptor = listBehaviors topicDescriptor.Topic
            member this.createMachine behaviorDescriptor = createMachine behaviorDescriptor.Behavior
    
    let build() = CoreLibrary() :> LibraryResolver
