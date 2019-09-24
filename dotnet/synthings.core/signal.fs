namespace synthings.core

type Signal =
    {
        Id : System.Guid;
        Epoch : System.DateTime;
        Time : System.DateTime;
        Value : float
    }

type Behavior = Signal -> Signal

module signal =
    open System
    open time
    
    let unpack (signal : Signal) = signal.Value
    
    let timeSpan (first : Signal) (second : Signal) = (second.Time - first.Time).TotalSeconds
    
    let secondsSinceEpoch (signal : Signal) = time.secondsSinceEpoch signal.Epoch signal.Time
    
    let createSignal (epoch : DateTime) (sampleTime : float) (value : float) =
        let dateTime = toDateTime epoch sampleTime
        {Id = Guid.NewGuid(); Epoch = epoch; Time = dateTime; Value = value}
    
    let createSignalSequence (startTime : float) (endTime : float) (interval : float) (value : float) =
        let epoch = time.now
        {startTime..interval..endTime}
        |> Seq.map (fun sampleTime -> createSignal epoch sampleTime value)
    
    let emptySequence : seq<Signal> = Seq.empty
    