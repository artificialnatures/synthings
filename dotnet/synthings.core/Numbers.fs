namespace synthings.core

module Numbers =
    open System

    let Epsilon = 0.000001;
    let DecimalPrecision = 5;
    let TwoPi = Math.PI * 2.0
    
    let square (a : float) =
        Math.Pow(a, 2.0)
    
    let equalWithinTolerance (a : float) (b : float) =
        let difference = a - b
        let absoluteDifference = Math.Abs difference
        absoluteDifference < Epsilon

    let normalizedPeriodicValue (period : float) (accumulatedValue : float) =
        match period, accumulatedValue with
        | 0.0, _ -> 0.0
        | _, 0.0 -> 0.0
        | _, _ when equalWithinTolerance period accumulatedValue -> 1.0
        | _, _ -> (accumulatedValue % period) / period
