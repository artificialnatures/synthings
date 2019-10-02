namespace synthings.core

type CoreTopic =
    | Core
    interface TopicIdentifier

type CoreBehavior =
    | Relay
    | UniformInteger
    | UniformFloat
    | Error
    interface BehaviorIdentifier

type CoreParameters =
    | Default
    | IntegerNumber of number : int
    | FloatNumber of number : float
    interface Parameters

module library =
    open System
    let internal relay (signal : Signal) = signal
    let internal uniformInteger (number : int) (signal : Signal) = {signal with Value = (float)number}
    let internal uniformFloat (number : float) (signal : Signal) = {signal with Value = number}
    let internal error (signal : Signal) = {signal with Value = 0.0} //TODO: add error handling
    
    let internal topics () =
        [
            {Topic = Core; DisplayName = "Core"; Id = Identifier.fromString("56DBAC92-F1A6-400C-8640-3C48CD10A2CC")}
        ]
    
    let internal behaviors =
        [
            {Behavior = Relay; DisplayName = "Relay"; Id = Identifier.fromString("F6D899DB-FF57-475D-94A1-2A1E0FB64DB4")};
            {Behavior = UniformInteger; DisplayName = "Uniform Integer"; Id = Identifier.fromString("E310E62F-251E-419B-B550-97EADBBF3066")};
            {Behavior = UniformFloat; DisplayName = "Uniform Float"; Id = Identifier.fromString("EFD34144-FCA8-4BCB-B877-8499FE07AD86")};
            {Behavior = Error; DisplayName = "Error"; Id = Identifier.fromString("2ACDA383-51D6-4946-8554-635ED1749C84")}
        ]
    
    let internal listBehaviors (topicIdentifier : TopicIdentifier) =
        match topicIdentifier with
        | :? CoreTopic as coreTopic ->
            match coreTopic with
            | Core -> behaviors
        | _ -> List.empty
    
    let internal createMachine (behaviorIdentifier : BehaviorIdentifier) =
        match behaviorIdentifier with
        | :? CoreBehavior as coreBehavior ->
            match coreBehavior with
            | Relay -> Machine.createMachine "Relay" relay
            | UniformInteger -> Machine.createMachine "Uniform Integer" (uniformInteger 1)
            | UniformFloat -> Machine.createMachine "Uniform Float" (uniformFloat 1.0)
            | Error -> Machine.createMachine "Error" error
        | _ -> Machine.createMachine "Error" error
    
type internal CoreLibrary() =
    static member build () = CoreLibrary() :> LibraryResolver
    interface LibraryResolver with
        member this.Origin = typeof<CoreTopic>.Namespace
        member this.Name = "Core"
        member this.listTopics () = library.topics()
        member this.listBehaviors topicDescriptor = library.listBehaviors topicDescriptor.Topic
        member this.createMachine behaviorDescriptor = library.createMachine behaviorDescriptor.Behavior
