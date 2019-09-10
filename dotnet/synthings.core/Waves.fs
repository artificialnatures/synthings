namespace synthings.core

module Waves =
    open System
    open Signals
    
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
        let increment = Numbers.normalizedPeriodicValue period time
        let x = increment * Numbers.TwoPi
        let y = Math.Sin x
        y * (amplitude / 2.0)
    
    let internal createSignal (signal : Signal) (value : float) =
        Some {signal with Value = value}
    
    let createWorker (configuration : Configuration) (signal : Signal) =
        let wave =
            match configuration.Wave with
            | Sine -> sine configuration.Period configuration.Amplitude
        signal
        |> secondsSinceEpoch
        |> wave
        |> createSignal signal
    
    let createMachine (configuration : Configuration) =
        createWorker configuration
        |> Machines.createMachine configuration.Name 
    