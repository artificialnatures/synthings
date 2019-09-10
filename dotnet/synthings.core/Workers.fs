namespace synthings.core

module Workers =
    open Signals
    
    type Worker = Signal -> Signal option

