namespace synthings.core

module wave =
    open System
    open signal
    
    type Wave =
        | Sine
    
    type Configuration =
        {
            Name : string;
            Wave : Wave;
            Period : float;
            Amplitude : float
        }
    
    let internal sine (period : float) (amplitude : float) (time : float) =
        let increment = number.normalizedPeriodicValue period time
        let x = increment * number.TwoPi
        let y = Math.Sin x
        y * (amplitude / 2.0)
    
    let internal createSignal (signal : Signal) (value : float) =
        {signal with Value = value}
    
    let createWorker (configuration : Configuration) (signal : Signal) =
        let wave =
            match configuration.Wave with
            | Sine -> sine configuration.Period configuration.Amplitude
        signal
        |> secondsSinceEpoch
        |> wave
        |> createSignal signal
    