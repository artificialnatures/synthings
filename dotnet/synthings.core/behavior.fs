namespace synthings.core

type Behavior = Signal -> Signal

(*
Create an extensible taxonomy for organizing behaviors:
Library (Core, Scalar, etc.)
Topic (Wave, Envelope, etc.)
Behavior (SineWave, LinearDecay, etc.)
Parameters (DefaultParameters, WaveParameters, etc.)
Each of these would be an empty marker interface and each library would implement that marker.
There still needs to be a way to turn one of these identifiers into actual data (parameters, functions, etc.)
Probably still a known module (e.g. "library") with functions for transforming identifiers into data.
createBehavior, createMachine, createParameters, etc. listTopics, listBehaviors, etc.
*)

module behavior =
    
    let relay (signal : Signal) = signal
    
    let error (signal : Signal) = {signal with Value = 0.0} //TODO: add error handling
