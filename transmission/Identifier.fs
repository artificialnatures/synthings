namespace synthings.transmission

type Identifier = System.Guid

module Identifier =
    let empty = System.Guid.Empty
    let create () : Identifier = System.Guid.NewGuid()

type InsertionOrder =
    | First
    | Last
    | Precede of Identifier

type EntityReference =
    | Root                              //The root entity in EntityTable
    | Ancestor of Identifier * int      //An entity int generations up the hierarchy from Identifier
    | Sibling of Identifier * int       //An entity int steps prior (-) or subsequent (+) to Identifier
    | Specified of Identifier           //A specific entity designated with Identifier