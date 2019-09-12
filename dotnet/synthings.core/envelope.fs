namespace synthings.core

module envelope =
    open signal
    
    type Envelope =
        | Linear
    
    type Configuration = {Name : string; Envelope : Envelope; Start : float; Duration : float}
    
    let linear (startTime : float) (duration : float) (sampleTime : float) =
        let slope = -1.0 / duration
        let increment = sampleTime - startTime
        let y = slope * increment + 1.0
        if y > 0.0 then y else 0.0
    
    let internal createSignal (signal : Signal) (scale : float) =
        let value = signal.Value * scale
        {signal with Value = value}
    
    let createWorker (configuration : Configuration) (signal : Signal) =
        let envelope =
            match configuration.Envelope with
            | Linear -> linear configuration.Start configuration.Duration
        signal
        |> secondsSinceEpoch
        |> time.since configuration.Start
        |> envelope
        |> createSignal signal
    