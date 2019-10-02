namespace synthings.core

type Signal =
    {
        Id : Identifier;
        Epoch : Instant;
        Time : Instant;
        Value : float
    }

type Signal with
    static member unpack (signal : Signal) = signal.Value
    static member timeSpan (earlier : Signal) (later : Signal) = (later.Time - earlier.Time).TotalSeconds
    static member secondsSinceEpoch (signal : Signal) = time.secondsSinceEpoch signal.Epoch signal.Time
    static member create (epoch : Instant) (sampleTime : Instant) (value : float) =
        {Id = identifier.create(); Epoch = epoch; Time = sampleTime; Value = value}
    static member createNow (epoch : Instant) (value : float) =
        let timeNow = time.now ()
        Signal.create epoch timeNow value
    static member createSample (epoch : Instant) (seconds : float) (value : float) =
        let sampleTime = epoch + System.TimeSpan.FromSeconds(seconds)
        Signal.create epoch sampleTime value
    static member createUniformSeries (epoch : Instant) (startTime : float) (endTime : float) (interval : float) (value : float) =
        [startTime .. interval .. endTime]
        |> List.map (fun sampleTime -> Signal.createSample epoch sampleTime value)
    static member createSeries (epoch : Instant) (startTime : float) (interval : float) (values : float list) =
        List.mapi (fun index value -> Signal.createSample epoch (startTime + ((float)index * interval)) value) values
    static member empty = Signal.createNow (time.now ()) 0.0
