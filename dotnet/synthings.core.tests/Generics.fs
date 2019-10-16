module synthings.core.tests.Generics

open Xunit

//Like this?

type SubjectType = interface end

type Word =
    | Word of string
    interface SubjectType

type Number =
    | Number of int
    interface SubjectType

type Change<'subject when 'subject :> SubjectType> =
    {
        Value : 'subject
    }

type ChangeType = interface end

type TestChange =
    | WordChange of Change<Word>
    | NumberChange of Change<Number>
    interface ChangeType

[<Fact>]
let ``Changes of different types`` () =
    let changes =
        [
            WordChange {Value = Word "this"}
            NumberChange {Value = Number 1}
            WordChange {Value = Word "that"}
            NumberChange {Value = Number 2}
            WordChange {Value = Word "the other"}
            NumberChange {Value = Number 3}
        ]
    let words = List.choose (fun change -> match change with | WordChange wordChange -> Some (string wordChange.Value) | _ -> None) changes
    Assert.Equal(3, words.Length)
