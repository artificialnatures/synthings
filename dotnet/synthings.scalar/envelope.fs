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
    open System
    let defaultStartTime = 0.0
    let defaultDuration = 10.0
    
    let linearDecay (startTime : float) (duration : float) =
        let line = curve.linear -1.0 duration 1.0
        let behavior (input : Signal) =
            let y =
                input
                |> Signal.secondsSinceEpoch
                //|> time.since startTime
                |> line
                |> number.positiveOrZero
                |> (*) input.Value
            {input with Value = y}
        behavior
    
    let displayName (behaviorIdentifier : EnvelopeBehavior) =
        match behaviorIdentifier with
        | LinearDecay -> "Linear Decay"
    
    let listBehaviors () =
        [
            {Behavior = LinearDecay; DisplayName = displayName LinearDecay; Id = Identifier.fromString("696AC296-A54D-4E1A-9942-F58C64FD14D1")}
        ]
    
    let createMachine (behaviorIdentifier : EnvelopeBehavior) (parameters : EnvelopeParameters) =
        let startTime, duration =
            match parameters with
            | Default -> defaultStartTime, defaultDuration
            | Specified (s, d) -> s, d
        let name = displayName behaviorIdentifier
        let behavior =
            match behaviorIdentifier with
            | LinearDecay -> linearDecay startTime duration
        Machine.createMachine name behavior
