namespace synthings.core

type Identifier = System.Guid

module identifier =
    let create () = System.Guid.NewGuid()
