namespace synthings.core

type Message =
    {
        Signal : Signal;
        Forwarder : System.Guid -> Message -> unit
    }

type Message with
    static member packWithoutForwarding signal = {Signal = signal; Forwarder = (fun (id : System.Guid) (message : Message) -> ())}
    static member pack signal forwarder = {Signal = signal; Forwarder = forwarder}
    static member unpack message = message.Signal
    static member repack message signal = {message with Signal = signal}
