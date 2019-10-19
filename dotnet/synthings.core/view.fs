namespace synthings.core

type View =
    {
        Id : Identifier;
        SubjectId : Identifier;
        DisplayName : string
    }

type View with
    static member create (subjectId : Identifier) (displayName : string) (window : Window) =
        {Id = Identifier.create(); SubjectId = subjectId; DisplayName = displayName}
    static member forMachine (machine : Machine) (window : Window) =
        View.create machine.Id machine.Name window
    static member forConnection (connection : Connection) =
        View.create connection.Id "Connection" Window.Empty
