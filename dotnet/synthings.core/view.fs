namespace synthings.core

type View =
    {
        Id : Identifier;
        DisplayName : string
    }

type View with
    static member forMachine (machine : Machine) =
        {Id = identifier.create(); DisplayName = machine.Name}
