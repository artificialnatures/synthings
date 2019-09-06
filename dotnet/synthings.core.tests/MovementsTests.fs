module synthings.core.tests.MovementsTests

open Xunit
open synthings.core

[<Fact>]
let ``Oscillate a vector`` () =
    let direction = Vectors.create 1.0 0.0 0.0
    let slowOscillate = Movements.oscillate 0.0 direction 1.0
    let result =
        seq {0.0 .. 0.25 .. 10.5}
        |> Seq.map (fun step -> slowOscillate step)
        |> Seq.map (fun vector -> Vectors.magnitude vector)
        |> Seq.filter (fun magnitude ->
            not(Numbers.equalWithinTolerance magnitude 0.0) &&
            not(Numbers.equalWithinTolerance magnitude 1.0))
        |> Seq.isEmpty
    Assert.True(result)

[<Fact>]
let ``Wiggle a vector`` () =
    let direction = Vectors.create 1.0 0.0 0.0
    let slowWiggle = Movements.wiggle 0.0 direction 1.0 10.0
    let result =
        seq {0.0 .. 0.25 .. 10.5}
        |> Seq.map (fun step -> slowWiggle step)
        |> Seq.map (fun vector -> Vectors.magnitude vector)
        |> Seq.filter (fun magnitude -> not(Numbers.equalWithinTolerance magnitude 0.0))
        |> Seq.pairwise
        |> Seq.forall (fun pair -> fst pair > snd pair)
    Assert.True(result)

