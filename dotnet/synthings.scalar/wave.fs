namespace synthings.scalar

module wave =
    open System
    open synthings.core
    
    let sineWave (period : float) (amplitude : float) =
        let behavior (input : Signal) =
            let y =
                input
                |> signal.secondsSinceEpoch
                |> number.normalizedPeriodicValue period
                |> (*) number.TwoPi
                |> Math.Sin
                |> (*) (amplitude / 2.0)
            {input with Value = y}
        behavior
    