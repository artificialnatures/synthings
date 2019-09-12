namespace synthings.core

module time =
    open System
    let now = DateTime.Now
    let since (referenceTime : float) (sampleTime : float) = sampleTime - referenceTime
    let until (referenceTime : float) (sampleTime : float) = sampleTime - referenceTime
    let toDateTime (epoch : DateTime) (time : float) = epoch + TimeSpan.FromSeconds time
    let secondsSinceEpoch (epoch : DateTime) (time : DateTime) = (time - epoch).TotalSeconds
