namespace synthings.scalar

open synthings.core

module scalarLibrary =
    type ScalarTopic =
        | Wave
            | SineWave
        | Envelope
            |LinearDecay
    
    type Parameters =
        | WaveParameters of period : float * amplitude : float
        | EnvelopeParameters of startTime : float * duration : float
    
    let internal identifiers =
        Map.empty
        |> Map.add Wave {Name = "Waves"; Id = System.Guid.Parse("2D70E7E1-677D-4474-9AF5-C416A4BAC3A9")}
        |> Map.add Envelope {Name = "Envelopes"; Id = System.Guid.Parse("9A5F0EB0-D0B7-41FC-AE53-FC21C752C1E3")}
        |> Map.add SineWave {Name = "Sine Wave"; Id = System.Guid.Parse("9463604B-74BF-47B8-860E-73048D6967F7")}
        |> Map.add LinearDecay {Name = "Linear Decay"; Id = System.Guid.Parse("80D4DCF5-9D82-4962-BBD9-F9B4FE7B23EC")}
    
    let internal reverseLookup =
        Map.toSeq identifiers
        |> Seq.map (fun pair -> (snd pair).Id, fst pair)
        |> Map.ofSeq
    
    let createBehavior behaviorType =
        match behaviorType with
        | SineWave -> wave.sineWave 1.0 2.0
        | LinearDecay -> envelope.linearDecay 0.0 10.0
        | _ -> behavior.error
    
    let createMachine (topic : ScalarTopic) (behaviorId : System.Guid) =
        let behaviorType = reverseLookup.Item behaviorId
        let name = (identifiers.Item behaviorType).Name
        let behavior = createBehavior behaviorType
        machine.createMachine name behavior
    
    let library =
        {Topics = [
            {
                Identifier = identifiers.Item Wave;
                BuildMachine = createMachine Wave;
                Behaviors = [identifiers.Item SineWave]
            };
            {
                Identifier = identifiers.Item Envelope;
                BuildMachine = createMachine Envelope;
                Behaviors = [identifiers.Item LinearDecay]
            }
        ]}

type ScalarLibrary() =
    interface LibraryModule with
        member this.Library = scalarLibrary.library
