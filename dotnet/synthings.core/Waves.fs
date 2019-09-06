namespace synthings.core

module Waves =
    open System

    let sine (period : float) (amplitude : float) (time : float) =
        let increment = Numbers.normalizedPeriodicValue period time
        let x = increment * Numbers.TwoPi
        let y = Math.Sin x
        y * (amplitude / 2.0)

    let sineDefault = sine 1.0 1.0
