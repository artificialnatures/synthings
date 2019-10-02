namespace synthings.core

type View =
    {
        Id : Identifier;
        DisplayName : string
    }

type View with
    static member forMachine (machine : Machine) =
        {Id = Identifier.create(); DisplayName = machine.Name}
