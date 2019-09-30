namespace synthings.core

type Monitor(window : Window) =
    let recording = {Signals = List.empty; Window = window}
    let recorder (signal : Signal) =
        Recording.append signal recording
        signal
    let machine = Machine.createMachine "Monitor" recorder
    member this.Recording = recording
    member this.LatestSignal = Recording.latestSignal recording
    member this.LatestValue = Recording.latestValue recording
    member this.Machine = machine
    static member createFrameWindowed (frames : int) = Monitor(FrameLimit(frames))
    static member createTimeWindowed (seconds : float) = Monitor(TimeLimit(seconds))
