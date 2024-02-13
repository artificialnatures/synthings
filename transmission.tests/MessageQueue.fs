module MessageQueue

open Expecto
open synthings.transmission

[<Tests>]
let tests =
  testList "MessageQueue" [
    testCase "All messages sent via channel can be received" <| fun _ ->
      let values = [1..10]
      let send, receive = MessageQueue.create<int, int> Channels
      List.iter (fun i -> send Identifier.empty i) values
      let receivedValues = 
        List.map (fun _ -> receive()) values
        |> List.filter Option.isSome
        |> List.map Option.get
        |> List.map (fun message -> message.proposal)
      Expect.sequenceEqual values receivedValues "All messages received in order."
  ]
