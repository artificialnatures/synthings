namespace synthings.scalar

module envelope =
    open synthings.core
    
    let linearDecay (startTime : float) (duration : float) =
        let line = curve.linear -1.0 duration 1.0
        let behavior (input : Signal) =
            let y =
                input
                |> signal.secondsSinceEpoch
                |> time.since startTime
                |> line
                |> number.positiveOrZero
                |> (*) input.Value
            {input with Value = y}
        behavior
    