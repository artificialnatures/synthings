module WavesTests
    
open Xunit
open synthings.core

[<Theory>]
[<InlineData(1.0, 0.0, 0.0)>]
[<InlineData(1.0, 0.25, 1.0)>]
[<InlineData(1.0, 0.5, 0.0)>]
[<InlineData(1.0, 0.75, -1.0)>]
[<InlineData(1.0, 1.0, 0.0)>]
let ``Sampling a sine wave`` period time expected =
    let scaledWave = Waves.sine period
    let actual = scaledWave time
    let result = Numbers.withinTolerance expected actual
    Assert.True(result)
