module synthings.core.tests.Changes

open Xunit
open synthings.core
(*
let changeSet =
    ChangeSet.empty ()
    |> ChangeSet.append (TextChange (Change.create (Text "this")))
    |> ChangeSet.append (QuantityChange (Change.create (Quantity 1)))
    |> ChangeSet.append (TextChange (Change.update (Text "that")))
    |> ChangeSet.append (QuantityChange (Change.update (Quantity 2)))
    |> ChangeSet.append (TextChange (Change.delete (Text "the other thing")))
    |> ChangeSet.append (QuantityChange (Change.delete (Quantity 3)))

let getQuantity (change : Change) =
    match change with
    | :? CoreChange as coreChange ->
        match coreChange with
        | QuantityChange quantityChange ->
            match quantityChange.Subject with
            | Quantity quantity -> Some quantity
        | _ -> None
    | _ -> None
let getText (change : Change) =
    match change with
    | :? CoreChange as coreChange ->
        match coreChange with
        | TextChange textChange ->
            match textChange.Subject with
            | Text text -> Some text
        | _ -> None
    | _ -> None

[<Fact>]
let ``ChangeSets may contain changes for a variety of Subjects`` () =
    let actualQuantity = List.choose getQuantity changeSet.Changes |> List.sum
    let actualText = List.choose getText changeSet.Changes |> String.concat ", "
    let expectedQuantity = 6
    let expectedText = "this, that, the other thing"
    Assert.Equal(expectedQuantity, actualQuantity)
    Assert.Equal(expectedText, actualText)
*)