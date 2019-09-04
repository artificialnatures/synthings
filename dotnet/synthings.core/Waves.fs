namespace synthings.core
open System

module Waves =
    let DefaultPeriod = Math.PI * 2.0
    
    let mapPeriod period time =
        time % period
    
    let sine period time =
        let increment = time % period
        let x = increment * DefaultPeriod
        Math.Sin x
    
