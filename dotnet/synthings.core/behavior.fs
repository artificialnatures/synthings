namespace synthings.core

type Behavior = Signal -> Signal

module behavior =
    
    let relay (signal : Signal) = signal
    
    let error (signal : Signal) = {signal with Value = 0.0} //TODO: add error handling
