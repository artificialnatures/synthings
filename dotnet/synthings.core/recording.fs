namespace synthings.core

type Window =
    | Empty
    | Singleton
    | FrameLimit of int
    | TimeLimit of float

type Recording =
    {
        Signals : Signal list;
        Window : Window
    }
    interface Subject

module window =
    let internal truncateByCount (limit : int) (signals : Signal list) =
        if signals.Length <= limit then signals else
            let startIndex = signals.Length - limit
            signals.[startIndex..]
    
    let internal findFirstIndexWithinLimit (limit : float) (signals : Signal list) =
        let latestTime = Signal.secondsSinceEpoch (List.last signals)
        let isWithinLimit (signal : Signal) =
            let timeSpan = latestTime - Signal.secondsSinceEpoch signal
            timeSpan <= limit
        List.findIndex isWithinLimit signals
    
    let internal truncateByTime (limit : float) (signals : Signal list) =
        if signals.Length = 0 then signals else
            let startIndex = findFirstIndexWithinLimit limit signals
            let truncated = signals.[startIndex..]
            truncated
    
    let internal limit (window : Window) (signals : Signal list) =
        match window with
        | Empty -> List.empty
        | Singleton -> truncateByCount 1 signals
        | FrameLimit limit -> truncateByCount limit signals
        | TimeLimit limit -> truncateByTime limit signals

type Recording with
    static member create (window : Window) = {Signals = List.empty; Window = window}
    static member append (signal : Signal) (recording : Recording) =
        let appended = List.append recording.Signals [signal]
        let limited = window.limit recording.Window appended
        {recording with Signals = limited}
    static member latestSignal (recording : Recording) =
        if List.isEmpty recording.Signals then Signal.empty else List.last recording.Signals
    static member latestValue (recording : Recording) =
        (Recording.latestSignal recording).Value
