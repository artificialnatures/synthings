module synthings.core.tests.WavesTests

open Xunit
open synthings.core

[<Theory>]
[<InlineData(4.0, 4.0, 0.0, 0.0)>]
[<InlineData(4.0, 4.0, 1.0, 2.0)>]
[<InlineData(4.0, 4.0, 2.0, 0.0)>]
[<InlineData(4.0, 4.0, 3.0, -2.0)>]
[<InlineData(4.0, 4.0, 4.0, 0.0)>]
let ``Sampling a custom sine wave`` period amplitude sampleTime expected =
    let configuration : wave.Configuration =
        {Name = "Sine Wave"; Wave = wave.Wave.Sine; Period = period; Amplitude = amplitude}
    let worker = wave.createWorker configuration
    let signal = signal.createSignal time.now sampleTime 0.0
    let outputSignal = worker signal
    Assert.Equal(expected, outputSignal.Value, number.DecimalPrecision)
