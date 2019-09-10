namespace synthings.core

module Envelopes =
    open Signals
    
    type Envelope =
        | Linear
    
    type Configuration = {Name : string; Envelope : Envelope; Start : float; Duration : float}
    
    let linear (startTime : float) (duration : float) (time : float) =
        let slope = -1.0 / duration
        let increment = time - startTime
        let y = slope * increment + 1.0
        if y > 0.0 then y else 0.0
    
    let internal createSignal (signal : Signal) (scale : float) =
        let value = signal.Value * scale
        Some {signal with Value = value}
    
    let createWorker (configuration : Configuration) (signal : Signal) =
        let envelope =
            match configuration.Envelope with
            | Linear -> linear configuration.Start configuration.Duration
        signal
        |> secondsSinceEpoch
        |> (-) configuration.Start
        |> envelope
        |> createSignal signal
    
    let createMachine (configuration : Configuration) =
        createWorker configuration
        |> Machines.createMachine configuration.Name
    