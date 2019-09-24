namespace synthings.core

module number =
    open System
    
    let Epsilon = 0.000001;
    let DecimalPrecision = 5;
    let TwoPi = Math.PI * 2.0
    
    let square (a : float) =
        Math.Pow(a, 2.0)
    
    let equalWithinTolerance (a : float) (b : float) =
        let difference = a - b
        let absoluteDifference = Math.Abs difference
        absoluteDifference < Epsilon
    
    let equalsZero (n : float) = equalWithinTolerance n 0.0
    
    let positiveOrZero (n : float) = if n > 0.0 then n else 0.0
    
    let equalsAny (validNumbers : float list) (value : float) =
        List.exists (fun validNumber -> equalWithinTolerance validNumber value) validNumbers
    
    let equalsNone (invalidNumbers : float list) (value : float) =
        not(equalsAny invalidNumbers value)
    
    let actualEqualsAnyInExpected (expected : float list) (actual : float list) =
        List.forall (fun a -> equalsAny expected a) actual
    
    let listsAreIdentical (expected : float list) (actual : float list) =
        if expected.Length <> actual.Length then false
        else
            List.zip expected actual
            |> List.forall (fun pair -> equalWithinTolerance (fst pair) (snd pair))
    
    let isDescending (values : float list) =
        List.pairwise values
        |> List.forall (fun pair -> fst pair > snd pair)
    
    let isAscending (values : float list) =
        List.pairwise values
        |> List.forall (fun pair -> fst pair < snd pair)
    
    let normalizedPeriodicValue (period : float) (accumulatedValue : float) =
        match period, accumulatedValue with
        | 0.0, _ -> 0.0
        | _, 0.0 -> 0.0
        | _, _ when equalWithinTolerance period accumulatedValue -> 1.0
        | _, _ -> (accumulatedValue % period) / period
