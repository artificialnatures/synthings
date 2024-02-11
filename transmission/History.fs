namespace synthings.transmission

type History<'entity> = ChangeSet<'entity> list

module History =
    let undo history =
        if List.isEmpty history
        then None, history
        else Some (ChangeSet.undo (List.head history)), List.tail history