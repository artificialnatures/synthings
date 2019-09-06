module synthings.core.tests.WavesTests
    
open Xunit
open synthings.core

[<Theory>]
[<InlineData(0.0, 0.0)>]
[<InlineData(0.25, 0.5)>]
[<InlineData(0.5, 0.0)>]
[<InlineData(0.75, -0.5)>]
[<InlineData(1.0, 0.0)>]
let ``Sampling a default sine wave`` time expected =
    let actual = Waves.sineDefault time
    let result = Numbers.equalWithinTolerance expected actual
    Assert.True(result)

[<Theory>]
[<InlineData(4.0, 4.0, 0.0, 0.0)>]
[<InlineData(4.0, 4.0, 1.0, 2.0)>]
[<InlineData(4.0, 4.0, 2.0, 0.0)>]
[<InlineData(4.0, 4.0, 3.0, -2.0)>]
[<InlineData(4.0, 4.0, 4.0, 0.0)>]
let ``Sampling a custom sine wave`` period amplitude time expected =
    let customSine = Waves.sine period amplitude
    let actual = customSine time
    Assert.Equal(expected, actual, Numbers.DecimalPrecision)
