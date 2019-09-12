module synthings.core.tests.NumbersTests

open Xunit
open synthings.core

[<Theory>]
[<InlineData(0.0, 0.0, 0.0)>]
[<InlineData(1.0, 0.0, 0.0)>]
[<InlineData(1.0, 0.5, 0.5)>]
[<InlineData(1.0, 1.5, 0.5)>]
[<InlineData(3.0, 0.0, 0.0)>]
[<InlineData(3.0, 1.0, 0.33333)>]
[<InlineData(3.0, 1.5, 0.5)>]
[<InlineData(3.0, 3.0, 1.0)>]
let ``Determining the current time increment`` period time expected =
    let actual = number.normalizedPeriodicValue period time
    Assert.Equal(expected, actual, number.DecimalPrecision)