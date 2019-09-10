namespace synthings.core

module Movements =
    let oscillate (amplitude : float) (frequency : float) (time : float) =
        let period = 1.0 / frequency
        let wave = Waves.sine period amplitude
        let increment = Numbers.normalizedPeriodicValue period time
        wave increment
    
    let wiggle (startTime : float) (amplitude : float) (frequency : float) (duration : float) (time : float) =
        let period = 1.0 / frequency
        let wave = Waves.sine period amplitude
        let decay = Envelopes.linear startTime duration
        wave time * decay time
    