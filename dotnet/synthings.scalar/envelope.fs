namespace synthings.scalar
open synthings.core

type EnvelopeParameters =
    | Default
    | Specified of startTime : float * duration : float
    interface Parameters

type EnvelopeBehavior =
    | LinearDecay
    interface BehaviorIdentifier

module envelope =
    let defaultStartTime = 0.0
    let defaultDuration = 10.0
    
    let linearDecay (startTime : Instant) (duration : float) =
        let line = curve.linear -1.0 duration 1.0
        let behavior (input : Signal) =
            match input with
            | :? CoreSignal as coreSignal ->
                match coreSignal with
                | DecimalSingleSignal signal ->
                    let y =
                        signal.Time
                        |> Instant.secondsBetween startTime
                        |> line
                        |> number.positiveOrZero
                        |> (*) (unbox signal.Value)
                    DecimalSingleSignal ({signal with Value = DecimalSingle(y)}) :> Signal
                | _ -> failwith "Incompatible input type."
            | _ -> failwith "Incompatible input type."
        behavior
    
    let displayName (behaviorIdentifier : EnvelopeBehavior) =
        match behaviorIdentifier with
        | LinearDecay -> "Linear Decay"
    
    let behaviorDescriptors =
        [
            {Behavior = LinearDecay; DisplayName = displayName LinearDecay; Id = Identifier.fromString("696AC296-A54D-4E1A-9942-F58C64FD14D1")}
        ]
    
    let findBehaviorDescriptor (behaviorIdentifier : BehaviorIdentifier) =
        List.find (fun (descriptor : BehaviorDescriptor) -> descriptor.Behavior = behaviorIdentifier) behaviorDescriptors
    
    let createMachine (behaviorIdentifier : EnvelopeBehavior) (parameters : EnvelopeParameters) =
        let startTime, duration =
            match parameters with
            | Default -> defaultStartTime, defaultDuration
            | Specified (s, d) -> s, d
        let name = displayName behaviorIdentifier
        let behavior =
            match behaviorIdentifier with
            | LinearDecay -> linearDecay (Instant.now()) duration
        Machine.createMachine name behavior
