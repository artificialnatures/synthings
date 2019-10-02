namespace synthings.core

type Instant =
    {
        Time : System.DateTime
    }

type Instant with
    static member fromDateTime (dateTime : System.DateTime) = {Time = dateTime}
    static member now () = {Time = System.DateTime.Now}
    static member secondsBetween (earlier : Instant) (later : Instant) =
        (later.Time - earlier.Time).TotalSeconds
    static member future (reference : Instant) (duration : float) =
        Instant.fromDateTime (reference.Time + System.TimeSpan.FromSeconds(duration))