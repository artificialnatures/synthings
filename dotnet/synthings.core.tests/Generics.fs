module synthings.core.tests.Generics

open Xunit

//Like this?

type SubjectType = interface end

type TestSubjectType =
    | WordType of word : string
    | NumberType of number : int
    interface SubjectType

type Subject = interface end

type TestSubject =
    | Words
    | Numbers
    interface Subject

type Change =
    {
        Value : SubjectType
    }

[<Fact>]
let ``Collections of different types`` () =
    let words =
        [
            {Value = WordType ("this")}
            {Value = WordType ("that")}
            {Value = WordType ("the other")}
        ]
    let numbers =
        [
            {Value = NumberType (1)}
            {Value = NumberType (2)}
            {Value = NumberType (3)}
        ]
    let changes (subject : Subject) =
        match subject with
        | :? TestSubject as testSubject ->
            match testSubject with
            | Words -> words
            | Numbers -> numbers
        | _ -> List.empty
    let retrievedWords = changes Words
    let retrievedNumbers = changes Numbers
    Assert.Equal(3, retrievedWords.Length)
    Assert.Equal(3, retrievedNumbers.Length)
