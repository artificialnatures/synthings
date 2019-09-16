namespace synthings.core

type Message =
    {
        Signal : Signal;
        Forwarder : System.Guid -> Message -> unit
    }

type Port = Message -> unit

type Forwarder = System.Guid -> Port

module message =
    let unpack message = message.Signal
    let repack message signal = {message with Signal = signal}
