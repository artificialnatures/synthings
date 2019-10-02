namespace synthings.core

type Identifier =
    {
        Guid : System.Guid
    }

type Identifier with
    static member create () = {Guid = System.Guid.NewGuid()}
    static member fromString (id : string) = {Guid = System.Guid.Parse(id)}
