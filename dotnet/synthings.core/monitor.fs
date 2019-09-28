namespace synthings.core

type Monitor() =
    let mutable recording = signal.empty
    //TODO: truncate based on time window, e.g. past 10 seconds
    let recorder (signal : Signal) =
        recording <- signal
        signal
    let machine = machine.createMachine "Monitor" recorder
    member this.Recording = recording
    member this.Length = 1
    member this.LatestSignal = recording
    member this.LatestValue = this.LatestSignal.Value
    member this.Machine = machine

module monitor =
    let create () = Monitor()
