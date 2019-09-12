namespace synthings.core

module movement =
    let oscillate (amplitude : float) (frequency : float) (time : float) =
        let period = 1.0 / frequency
        let wave = wave.sine period amplitude
        let increment = number.normalizedPeriodicValue period time
        wave increment
    
    let wiggle (startTime : float) (amplitude : float) (frequency : float) (duration : float) (time : float) =
        let period = 1.0 / frequency
        let wave = wave.sine period amplitude
        let decay = envelope.linear startTime duration
        wave time * decay time
    