namespace synthings.core

type Window =
    | FrameLimit of int
    | TimeLimit of float

type Recording =
    {
        mutable Signals : Signal list;
        Window : Window
    }

module window =
    let truncateByCount (limit : int) (signals : Signal list) =
        if signals.Length <= limit then signals else
            let startIndex = signals.Length - limit
            signals.[startIndex..]
    
    let findFirstIndexWithinLimit (limit : float) (signals : Signal list) =
        let latestTime = Signal.secondsSinceEpoch (List.last signals)
        let isWithinLimit (signal : Signal) =
            let timeSpan = latestTime - Signal.secondsSinceEpoch signal
            timeSpan <= limit
        List.findIndex isWithinLimit signals
    
    let truncateByTime (limit : float) (signals : Signal list) =
        if signals.Length = 0 then signals else
            let startIndex = findFirstIndexWithinLimit limit signals
            signals.[startIndex..]
    
    let limit (window : Window) (signals : Signal list) =
        match window with
        | FrameLimit limit -> truncateByCount limit signals
        | TimeLimit limit -> truncateByTime limit signals

type Recording with
    static member append (signal : Signal) (recording : Recording) =
        recording.Signals <- window.limit recording.Window (List.append recording.Signals [signal])
    
    static member latestSignal (recording : Recording) =
        if List.isEmpty recording.Signals then Signal.empty else List.last recording.Signals
    
    static member latestValue (recording : Recording) =
        (Recording.latestSignal recording).Value
