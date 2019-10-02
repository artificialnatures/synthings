namespace synthings.core

type Message =
    {
        Signal : Signal;
        Forwarder : Identifier -> Message -> unit
    }

type Message with
    static member packWithoutForwarding signal = {Signal = signal; Forwarder = (fun (id : Identifier) (message : Message) -> ())}
    static member pack signal forwarder = {Signal = signal; Forwarder = forwarder}
    static member unpack message = message.Signal
    static member repack message signal = {message with Signal = signal}
