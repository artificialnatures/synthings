namespace synthings.core
open System

module Numbers =
    let Epsilon = 0.000001;
    
    let withinTolerance a b =
        let difference = (float) a - b
        let absoluteDifference = Math.Abs difference
        absoluteDifference < Epsilon
