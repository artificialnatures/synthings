module synthings.core.tests.WavesTests
    
open System
open Xunit
open synthings.core

[<Theory>]
[<InlineData(4.0, 4.0, 0.0, 0.0)>]
[<InlineData(4.0, 4.0, 1.0, 2.0)>]
[<InlineData(4.0, 4.0, 2.0, 0.0)>]
[<InlineData(4.0, 4.0, 3.0, -2.0)>]
[<InlineData(4.0, 4.0, 4.0, 0.0)>]
let ``Sampling a custom sine wave`` period amplitude time expected =
    let configuration : Waves.Configuration =
        {Name = "Sine Wave"; Wave = Waves.Wave.Sine; Period = period; Amplitude = amplitude}
    let worker = Waves.createWorker configuration
    let signal = Signals.createTimeSignal DateTime.Now time
    let outputSignal = worker signal |> Option.get
    Assert.Equal(expected, outputSignal.Value, Numbers.DecimalPrecision)
