namespace synthings.core

module Vectors =
    open System.Numerics
    
    let create (x : float) (y : float) (z : float) =
        new Vector3((float32 x), (float32 y), (float32 z))
    
    let magnitude (vector : Vector3) =
        let length = vector.Length()
        (float length)
    
    let scale (vector : Vector3) (scalar : float) =
        Vector3.Multiply(vector, (float32 scalar))
    