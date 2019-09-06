namespace synthings.core

module Movements =
    open System.Numerics
    
    let oscillate (startTime : float) (direction : Vector3) (frequency : float) (time : float) =
        let period = 1.0 / frequency
        let magnitude = Vectors.magnitude direction
        let amplitude = magnitude * 2.0
        let wave = Waves.sine period amplitude
        let scale = wave time
        Vectors.scale direction scale
    
    let wiggle (startTime : float) (direction : Vector3) (frequency : float) (duration : float) (time : float) =
        let period = 1.0 / frequency
        let magnitude = Vectors.magnitude direction
        let amplitude = magnitude * 2.0
        let wave = Waves.sine period amplitude
        let decay = Decay.linear startTime duration
        let scale = wave time * decay time
        Vectors.scale direction scale
    