namespace synthings.core

type Message =
    {
        Signal : Signal;
        Forwarder : System.Guid -> Message -> unit
    }

type Port = Message -> unit

type Forwarder = System.Guid -> Port

module message =
    let nullForwarder (id : System.Guid) = (fun (message : Message) -> ())
    let packWithoutForwarding signal = {Signal = signal; Forwarder = nullForwarder}
    let pack signal forwarder = {Signal = signal; Forwarder = forwarder}
    let unpack message = message.Signal
    let repack message signal = {message with Signal = signal}
