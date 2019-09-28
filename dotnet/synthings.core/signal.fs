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
    
    let createRealtimeSignal (epoch : DateTime) (originationTime : DateTime) (value : float) =
        createSignal epoch (originationTime - epoch).TotalSeconds value
    
    let empty = createSignal (time.now()) 0.0 0.0
    
    let createUniformSignalSequence (startTime : float) (endTime : float) (interval : float) (value : float) =
        let epoch = time.now()
        {startTime..interval..endTime}
        |> Seq.map (fun sampleTime -> createSignal epoch sampleTime value)
    
    let createSignalSequence (startTime : float) (interval : float) (values : float list) =
        let epoch = time.now()
        let endTime = interval * (float)(List.length values)
        let sampleTimes = {startTime .. interval .. endTime}
        Seq.zip sampleTimes values
        |> Seq.map (fun pair -> createSignal epoch (fst pair) (snd pair))
    
    let emptySequence : seq<Signal> = Seq.empty
    