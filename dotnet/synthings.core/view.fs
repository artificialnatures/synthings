namespace synthings.core

type View =
    {
        Id : Identifier;
        SubjectId : Identifier;
        DisplayName : string;
        History : seq<Signal>
    }

type View with
    static member create (subjectId : Identifier) (displayName : string) =
        {Id = Identifier.create(); SubjectId = subjectId; DisplayName = displayName; History = Seq.empty}
    static member forMachine (machine : Machine) =
        View.create machine.Id machine.Name
    static member forConnection (connection : Connection) =
        View.create connection.Id "Connection"
