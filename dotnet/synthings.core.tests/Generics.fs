module synthings.core.tests.Generics

open Xunit

//Like this?

type SubjectType = interface end

type Word =
    | Word of string
    interface SubjectType

type Number =
    | Number of float
    interface SubjectType

type WordNumber = {Text : string; Amount : float} interface SubjectType

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
            NumberChange {Value = Number 1.0}
            WordChange {Value = Word "that"}
            NumberChange {Value = Number 2.0}
            WordChange {Value = Word "the other"}
            NumberChange {Value = Number 3.0}
        ]
    let chooseWord change =
        match change with
        | WordChange wordChange -> Some (string wordChange.Value)
        | _ -> None
    let strings = List.choose chooseWord changes
    let chooseNumber change =
        match change with
        | NumberChange numberChange -> Some (float numberChange.Value) //Why?
        | _ -> None
    let ints = List.choose chooseNumber changes
    Assert.Equal(3, strings.Length)
    Assert.Equal(3, ints.Length)
