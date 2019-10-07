namespace synthings.core

type View =
    {
        Id : Identifier;
        SubjectId : Identifier;
        DisplayName : string;
        Recording : Recording
    }

type View with
    static member create (subjectId : Identifier) (displayName : string) (window : Window) =
        {Id = Identifier.create(); SubjectId = subjectId; DisplayName = displayName; Recording = Recording.create window}
    static member forMachine (machine : Machine) (window : Window) =
        View.create machine.Id machine.Name window
    static member forConnection (connection : Connection) =
        View.create connection.Id "Connection" Window.Empty
    static member record (signal : Signal) (view : View) =
        let recording = Recording.append signal view.Recording
        {view with Recording = recording}
