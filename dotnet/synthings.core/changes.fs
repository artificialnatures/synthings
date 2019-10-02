namespace synthings.core

type Operation =
    | Create
    | Update
    | Delete

type Change<'subjectType> =
    {
        Operation : Operation;
        Subject : 'subjectType
    }

type Change<'subjectType> with
    static member create (subject : 'subjectType) =
        {Operation = Create; Subject = subject}
    static member update (subject : 'subjectType) =
        {Operation = Update; Subject = subject}
    static member delete (subject : 'subjectType) =
        {Operation = Delete; Subject = subject}

type ChangeSet =
    {
        Id : Identifier;
        TimeOfCreation : Instant;
        MachineChanges : Change<Machine> list;
        ViewChanges : Change<View> list
    }

type ChangeSet with
    static member withChangeLists (machineChanges : Change<Machine> list) (viewChanges : Change<View> list) =
        {Id = Identifier.create(); TimeOfCreation = Instant.now(); MachineChanges = machineChanges; ViewChanges = viewChanges}
    static member machineCreated (machine : Machine) (view : View) =
        ChangeSet.withChangeLists [Change.create machine] [Change.create view]
