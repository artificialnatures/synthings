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
        MachineChanges : Change<Machine> list;
        ConnectionChanges : Change<Connection> list;
        ViewChanges : Change<View> list
    }

type ChangeSet with
    static member withChangeLists (machineChanges : Change<Machine> list) (connectionChanges : Change<Connection> list) (viewChanges : Change<View> list) =
        {Id = Identifier.create(); TimeOfCreation = Instant.now(); MachineChanges = machineChanges; ConnectionChanges = connectionChanges; ViewChanges = viewChanges}
    static member empty = ChangeSet.withChangeLists List.empty List.empty List.empty
    static member machineCreated (machine : Machine) (view : View) =
        ChangeSet.withChangeLists [Change.create machine] List.empty [Change.create view]
    static member connectionCreated (connection : Connection) (view : View) =
        ChangeSet.withChangeLists List.empty [Change.create connection] [Change.create view]
    static member addView (view : View) (changeSet : ChangeSet) =
        {changeSet with ViewChanges = List.append changeSet.ViewChanges [Change.update view]}
