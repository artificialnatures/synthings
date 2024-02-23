namespace synthings.ui.console

module SynthingsConsoleApplication =
    open synthings.transmission
    let create () =
        let configuration =
            {
                messagingImplementation = Channels
            }
        let renderer = ConsoleRenderer.create ()
        Application(configuration, ApplicationContainer, ConsoleRenderer.create)
