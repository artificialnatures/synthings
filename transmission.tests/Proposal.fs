module Proposal

open Microsoft.FSharp.Reflection
open Expecto
open synthings.transmission

[<Tests>]
let tests =
    testList "Proposal" [
        testCase "Proposal specifies changes to apply to the application state" <| fun _ ->
            let ``should be a valid entity id`` = Identifier.create()
            let proposals =
                [
                    Initialize {
                        initialTree = Leaf 1
                    }
                    Add {
                        parent = Specified ``should be a valid entity id`` //should be the Identifier of the parent of entity
                        order = Last //the sibling that entity will come before, if None entity will be inserted at the end of the children list
                        entityToAdd = Leaf 1 //the entity to create, in tree form. children will 
                    }
                    Replace {
                        entityToReplace = Specified ``should be a valid entity id``
                        replacement = Node (1, [Leaf 2])
                    }
                    Remove {
                        entityToRemove = Specified ``should be a valid entity id`` //the entity with entityId will be deleted, along with all of it's children
                    }
                ]
            Expect.equal (List.length proposals) (Array.length (FSharpType.GetUnionCases(typeof<Proposal<_>>))) "Not all Proposal cases are covered."
    ]
