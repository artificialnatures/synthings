namespace synthings.scalar

module wave =
    open System
    open synthings.core.number
    open synthings.core.signal
    
    let sineWave (period : float) (amplitude : float) =
        let behavior (signal : Signal) =
            let y =
                signal
                |> secondsSinceEpoch
                |> normalizedPeriodicValue period
                |> (*) TwoPi
                |> Math.Sin
                |> (*) (amplitude / 2.0)
            {signal with Value = y}
        behavior
    