namespace synthings.core

type View =
    {
        Id : System.Guid;
        DisplayName : string
    }

type View with
    static member forMachine (machine : Machine) =
        {Id = System.Guid.NewGuid(); DisplayName = machine.Name}
