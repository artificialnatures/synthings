namespace synthings.core

type Signal<'signal when 'signal :> SignalValue> =
    {
        Epoch : Instant;
        Time : Instant;
        Value : 'signal
    }

type SignalSequenceConfiguration =
    {
        Epoch : Instant;
        StartAtSeconds : float;
        EndAtSeconds : float;
        IntervalSeconds : float
    }

type Signal<'signal when 'signal :> SignalValue> with
    static member unpack signal = signal.Value
    static member timeSpan earlier later = Instant.secondsBetween earlier.Time later.Time
    static member secondsSinceEpoch signal = Instant.secondsBetween signal.Epoch signal.Time
    static member create epoch sampleTime value =
        {Epoch = epoch; Time = sampleTime; Value = value}
    static member createFromInput signal value =
        {Epoch = signal.Epoch; Time = signal.Time; Value = value}
    static member createNow epoch value =
        let timeNow = Instant.now ()
        Signal.create epoch timeNow value
    static member createSample epoch seconds value =
        let sampleTime = Instant.future epoch seconds
        Signal.create epoch sampleTime value
    static member createUniformSeries configuration value =
        [configuration.StartAtSeconds .. configuration.IntervalSeconds .. configuration.EndAtSeconds]
        |> List.map (fun sampleTime -> Signal.createSample configuration.Epoch sampleTime value)
    static member createSeries epoch startTime interval values =
        List.mapi (fun index value -> Signal.createSample epoch (startTime + ((float)index * interval)) value) values
