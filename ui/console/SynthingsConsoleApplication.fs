namespace synthings.ui.console

module SynthingsConsoleApplication =
    open synthings.transmission
    let build () =
        let transmission = Transmission()
        transmission.WithRenderer ConsoleRenderer.render None
        transmission
