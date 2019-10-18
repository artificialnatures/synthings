namespace synthings.core

//TODO: Maybe Operation is Event?
type Operation =
    | Create
    | Update
    | Delete

type Change<'subject when 'subject :> Subject> =
    {
        Operation : Operation;
        Subject : 'subject
    }

type Change<'subject when 'subject :> Subject> with
    static member create subject =
        {Operation = Create; Subject = subject}
    static member update subject =
        {Operation = Update; Subject = subject}
    static member delete subject =
        {Operation = Delete; Subject = subject}
    static member unboxSubject (change : Change<'subject>) =
        unbox change.Subject

type ChangeSet =
    {
        Id : Identifier;
        TimeOfCreation : Instant;
        Changes : Change list;
    }

type ChangeSet with
    static member empty () =
        {Id = Identifier.create(); TimeOfCreation = Instant.now(); Changes = List.empty}
    static member ofList (changes : Change list) =
        {ChangeSet.empty() with Changes = changes}
    static member append (change : Change) (changeSet : ChangeSet) =
        {changeSet with Changes = (List.append changeSet.Changes [change])}
