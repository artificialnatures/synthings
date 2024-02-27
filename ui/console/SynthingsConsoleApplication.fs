namespace synthings.ui.console

module SynthingsConsoleApplication =
    open synthings.transmission
    let create () =
        let application = Application()
        application.WithRenderer ConsoleRenderer.render None
        application
