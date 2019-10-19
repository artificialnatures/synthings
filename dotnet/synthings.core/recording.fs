namespace synthings.core

type Window =
    | Empty
    | Singleton
    | FrameLimit of int
    | TimeLimit of float

type Recording<'signal when 'signal :> SignalValue> =
    {
        Signals : Signal<'signal> list;
        Window : Window
    }
    interface SignalValue

module window =
    let internal truncateByCount limit (signals : Signal<_> list) =
        if signals.Length <= limit then signals else
            let startIndex = signals.Length - limit
            signals.[startIndex..]
    
    let internal findFirstIndexWithinLimit limit signals =
        let latestTime = Signal.secondsSinceEpoch (List.last signals)
        let isWithinLimit signal =
            let timeSpan = latestTime - Signal.secondsSinceEpoch signal
            timeSpan <= limit
        List.findIndex isWithinLimit signals
    
    let internal truncateByTime limit signals =
        if List.isEmpty signals then signals else
            let startIndex = findFirstIndexWithinLimit limit signals
            let truncated = signals.[startIndex..]
            truncated
    
    let internal limit window signals =
        match window with
        | Empty -> List.empty
        | Singleton -> truncateByCount 1 signals
        | FrameLimit limit -> truncateByCount limit signals
        | TimeLimit limit -> truncateByTime limit signals

type Recording<'signal when 'signal :> SignalValue> with
    static member create window = {Signals = List.empty; Window = window}
    static member append signal recording =
        let appended = List.append recording.Signals [signal]
        let limited = window.limit recording.Window appended
        {recording with Signals = limited}
    static member latestSignal recording =
        if List.isEmpty recording.Signals then None else Some (List.last recording.Signals)
