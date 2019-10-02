namespace synthings.core

type Instant = System.DateTime

module time =
    let now () = System.DateTime.Now
    let since (referenceTime : float) (sampleTime : float) = sampleTime - referenceTime
    let until (referenceTime : float) (sampleTime : float) = sampleTime - referenceTime
    let toDateTime (epoch : Instant) (time : float) = epoch + System.TimeSpan.FromSeconds time
    let secondsSinceEpoch (epoch : Instant) (time : Instant) = (time - epoch).TotalSeconds
