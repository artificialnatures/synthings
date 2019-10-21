namespace synthings.scalar

open synthings.core

type WaveParameters =
    | Default
    | Specified of period : float * amplitude : float
    interface Parameters

type WaveBehavior =
    | SineWave
    interface BehaviorIdentifier

module wave =
    open System
    
    let defaultPeriod = 1.0
    let defaultAmplitude = 2.0
    
    let sineWave (period : float) (amplitude : float) =
        let behavior (input : Signal) =
            match input with
            | :? CoreSignal as coreSignal ->
                match coreSignal with
                | EmptySignal signal ->
                    let y =
                        signal
                        |> Signal.secondsSinceEpoch
                        |> number.normalizedPeriodicValue period
                        |> (*) number.TwoPi
                        |> Math.Sin
                        |> (*) (amplitude / 2.0)
                    DecimalSingleSignal (Signal.createFromInput signal (DecimalSingle(y))) :> Signal
                | _ -> failwith "Incompatible input type."
            | _ -> failwith "Incompatible input type."
        behavior
    
    let displayName (behaviorIdentifier : WaveBehavior) =
        match behaviorIdentifier with
        | SineWave -> "Sine Wave"
    
    let behaviorDescriptors =
        [
            {Behavior = SineWave; DisplayName = displayName SineWave; Id = Identifier.fromString("F681ED1F-0FD2-4763-9215-216323CE7CB7")}
        ]
    
    let findBehaviorDescriptor (behaviorIdentifier : BehaviorIdentifier) =
        List.find (fun (descriptor : BehaviorDescriptor) -> descriptor.Behavior = behaviorIdentifier) behaviorDescriptors
    
    let createMachine (behaviorIdentifier : WaveBehavior) (parameters : WaveParameters) =
        let period, amplitude =
            match parameters with
            | Default -> defaultPeriod, defaultAmplitude
            | Specified (p, a) -> p, a
        let behavior =
            match behaviorIdentifier with
            | SineWave -> sineWave period amplitude
        let name = displayName behaviorIdentifier
        Machine.createMachine name behavior
