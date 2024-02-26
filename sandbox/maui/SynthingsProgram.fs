namespace synthings.sandbox.maui

open synthings.transmission
open synthings.ui.maui

module SynthingsProgram =
    let buildApplication () =
        let application = Application()
        application.WithRenderer (MauiRenderer.create ())
        application
