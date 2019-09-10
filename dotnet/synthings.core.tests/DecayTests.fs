module synthings.core.tests.DecayTests

open Xunit
open synthings.core

[<Theory>]
[<InlineData(0.0, 1.0, 0.0, 1.0)>]
[<InlineData(0.0, 1.0, 0.5, 0.5)>]
[<InlineData(0.0, 1.0, 1.0, 0.0)>]
[<InlineData(0.0, 1.0, 1.5, 0.0)>]
[<InlineData(3.0, 5.0, 3.0, 1.0)>]
[<InlineData(3.0, 5.0, 5.5, 0.5)>]
[<InlineData(3.0, 5.0, 8.0, 0.0)>]
[<InlineData(3.0, 5.0, 10.0, 0.0)>]
let ``Linear decay`` startTime duration time expected =
    let actual = Envelopes.linear startTime duration time
    Assert.Equal(expected, actual, Numbers.DecimalPrecision)

