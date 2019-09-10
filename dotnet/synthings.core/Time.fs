namespace synthings.core
open System

module Time =
    let now = DateTime.Now
    
    let toDateTime (epoch : DateTime) (time : float) =
        epoch + TimeSpan.FromSeconds time
    
    let secondsSinceEpoch (epoch : DateTime) (time : DateTime) =
        (time - epoch).TotalSeconds
