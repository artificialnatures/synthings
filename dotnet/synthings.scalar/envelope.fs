namespace synthings.scalar

module envelope =
    open synthings.core
    open synthings.core.signal
    
    let linearDecay (startTime : float) (duration : float) =
        let line = curve.linear -1.0 duration 1.0
        let behavior (signal : Signal) =
            let y =
                signal
                |> secondsSinceEpoch
                |> time.since startTime
                |> line
                |> number.positiveOrZero
                |> (*) signal.Value
            {signal with Value = y}
        behavior
    