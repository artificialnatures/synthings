namespace synthings.core

type Operation =
    | Create
    | Update
    | Delete

type Change<'subject> =
    {
        Operation : Operation;
        Subject : 'subject
    }

type Change<'subject> with
    static member create (subject : 'subject) =
        {Operation = Create; Subject = subject}
    static member update (subject : 'subject) =
        {Operation = Update; Subject = subject}
    static member delete (subject : 'subject) =
        {Operation = Delete; Subject = subject}

type ChangeSet =
    {
        Id : Identifier;
        TimeOfCreation : Instant;
        Subjects : Subject list;
    }

module changes =
    let placeholderTypeResolver (subject : Subject) =
        match subject with
        | :? CoreSubject as coreSubject ->
            match coreSubject with
            | Graph -> typeof<Graph>
            | Machine -> typeof<Machine>
            | Connection -> typeof<Connection>
            | Recording -> typeof<Recording>
        | _ -> typeof<float>
    let subjectDescriptors =
        [
            {Subject = Graph; SubjectType = typeof<Graph>; DisplayName = "Graph"; Id = Identifier.create()};
            {Subject = Machine; SubjectType = typeof<Machine>; DisplayName = "Machine"; Id = Identifier.create()};
            {Subject = Connection; SubjectType = typeof<Connection>; DisplayName = "Connection"; Id = Identifier.create()};
            {Subject = Recording; SubjectType = typeof<Recording>; DisplayName = "Recording"; Id = Identifier.create()};
        ]

type ChangeSet with
    static member empty = {Id = Identifier.create(); TimeOfCreation = Instant.now(); Subjects = List.empty}
    static member changesBySubject (subject : Subject) (changeSet : ChangeSet) =
        0
    static member append subject (changeSet : ChangeSet) = changeSet
