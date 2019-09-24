module synthings.library.tests.Libraries

open Xunit
open synthings.core

[<Fact>]
let ``Load libraries`` () =
    //Test every single function in aggregateLibrary...
    let libraries = aggregateLibrary.findAssemblies()
    let resolvers = Seq.map (fun lib -> aggregateLibrary.findLibraryResolvers lib) libraries
    Assert.True(Seq.length libraries > 0)
