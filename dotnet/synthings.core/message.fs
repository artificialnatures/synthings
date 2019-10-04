namespace synthings.core

type Forwarder = Identifier -> Message -> unit
and Message =
    {
        Signal : Signal;
        Forwarder : Forwarder
    }

type Message with
    static member packWithoutForwarding signal = {Signal = signal; Forwarder = (fun (id : Identifier) (message : Message) -> ())}
    static member pack signal forwarder = {Signal = signal; Forwarder = forwarder}
    static member unpack message = message.Signal
    static member repack message signal = {message with Signal = signal}
