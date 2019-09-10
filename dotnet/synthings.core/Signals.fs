namespace synthings.core
open System

module Signals =
    type Signal =
        {
            Id : Guid;
            Epoch : DateTime;
            Time : DateTime;
            Value : float
        }
    
    let secondsSinceEpoch (signal : Signal) = Time.secondsSinceEpoch signal.Epoch signal.Time
    
    let createTimeSignal (epoch : DateTime) (time : float) =
        let sampleTime = Time.toDateTime epoch time
        {Id = Guid.NewGuid(); Epoch = epoch; Time = sampleTime; Value = 0.0}
    
    let createTimeSignals (times : seq<float>) =
        let epochSignal = createTimeSignal Time.now
        Seq.map epochSignal times
    
    let emptySequence : seq<Signal> = Seq.empty
    