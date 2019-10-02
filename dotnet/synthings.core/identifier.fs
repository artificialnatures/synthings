namespace synthings.core

type Identifier = System.Guid

module identifier =
    let create () = System.Guid.NewGuid()
    let fromString (id : string) = System.Guid.Parse(id)
