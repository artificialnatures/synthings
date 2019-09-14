namespace synthings.scalar

module library =
    open synthings.core
    
    type Scalar =
        | Wave
            | SineWave
        | Envelope
            |LinearDecay
    
    type Parameters =
        | WaveParameters of period : float * amplitude : float
        | EnvelopeParameters of startTime : float * duration : float
    
    let createBehavior behaviorType parameters =
        match behaviorType with
        | Wave
            | SineWave ->
                match parameters with
                | WaveParameters (period, amplitude) -> wave.sineWave period amplitude
                | _ -> behavior.error
        | Envelope
            | LinearDecay ->
                match parameters with
                | EnvelopeParameters (startTime, duration) -> envelope.linearDecay startTime duration
                | _ -> behavior.error
