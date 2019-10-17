module synthings.core.tests.Generics

open Xunit

//In F# generic types can be combined with marker (empty) interfaces
//and discriminated unions to enable bounded generic handling of
//data types by pattern matching.
//The logic is:
// 1. box(box(box(data))) //each box corresponds to a layer of abstraction: data -> generic type -> discriminated union case -> marker interface type
// 2. collect the wrapped data into a list of state changes
// 3. match on library -> match on grouping -> match on data type -> data //unbox the data to use it on the consumer side

type Subject = interface end

type Word =
    | Word of string
    interface Subject

type Number =
    | Number of float
    interface Subject

type WordNumber = {Text : string; Amount : float} interface Subject

type Change<'subject when 'subject :> Subject> =
    {
        Value : 'subject
    }

type ChangeType = interface end

type TestChange =
    | WordChange of Change<Word>
    | NumberChange of Change<Number>
    | WordNumberChange of Change<WordNumber>
    interface ChangeType

[<Fact>]
let ``Generic Changes`` () =
    let changes =
        [
            WordChange {Value = Word "this"}
            NumberChange {Value = Number 1.0}
            WordChange {Value = Word "that"}
            NumberChange {Value = Number 2.0}
            WordChange {Value = Word "the other"}
            NumberChange {Value = Number 3.0}
            WordNumberChange {Value = {Text = "Word"; Amount = 10.0}}
        ]
    let chooseWord change =
        match change with
        | WordChange wordChange -> Some wordChange.Value
        | _ -> None
    let strings = List.choose chooseWord changes
    let chooseNumber change =
        match change with
        | NumberChange numberChange -> Some numberChange.Value
        | _ -> None
    let ints = List.choose chooseNumber changes
    let chooseWordNumber change =
        match change with
        | WordNumberChange wordNumberChange -> Some wordNumberChange.Value
        | _ -> None
    let wordNumbers = List.choose chooseWordNumber changes
    Assert.Equal(3, strings.Length)
    Assert.Equal(3, ints.Length)
    Assert.Equal(1, wordNumbers.Length)
