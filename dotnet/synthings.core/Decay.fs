namespace synthings.core

module Decay =
    let linear (startTime : float) (duration : float) (time : float) =
        let slope = -1.0 / duration
        let increment = time - startTime
        let y = slope * increment + 1.0
        if y > 0.0 then y else 0.0
    