namespace synthings.core

//TODO: Make Signal generic
type Signal =
    {
        Id : Identifier;
        Epoch : Instant;
        Time : Instant;
        Value : float
    }

type Signal with
    static member unpack (signal : Signal) = signal.Value
    static member timeSpan (earlier : Signal) (later : Signal) = Instant.secondsBetween earlier.Time later.Time
    static member secondsSinceEpoch (signal : Signal) = Instant.secondsBetween signal.Epoch signal.Time
    static member create (epoch : Instant) (sampleTime : Instant) (value : float) =
        {Id = Identifier.create(); Epoch = epoch; Time = sampleTime; Value = value}
    static member createNow (epoch : Instant) (value : float) =
        let timeNow = Instant.now ()
        Signal.create epoch timeNow value
    static member createSample (epoch : Instant) (seconds : float) (value : float) =
        let sampleTime = Instant.future epoch seconds
        Signal.create epoch sampleTime value
    static member createUniformSeries (epoch : Instant) (startTime : float) (endTime : float) (interval : float) (value : float) =
        [startTime .. interval .. endTime]
        |> List.map (fun sampleTime -> Signal.createSample epoch sampleTime value)
    static member createSeries (epoch : Instant) (startTime : float) (interval : float) (values : float list) =
        List.mapi (fun index value -> Signal.createSample epoch (startTime + ((float)index * interval)) value) values
    static member empty = Signal.createNow (Instant.now ()) 0.0
