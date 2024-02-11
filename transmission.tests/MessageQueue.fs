module MessageQueue

open Expecto
open synthings.transmission

[<Tests>]
let tests =
  testList "MessageQueue" [
    testCase "All messages sent via channel can be received" <| fun _ ->
      let messages = [1..10]
      let send, receive = MessageQueue.create<int, int> Channels
      List.iter send messages
      let receivedMessages = 
        List.map (fun _ -> receive()) messages
        |> List.filter Option.isSome
        |> List.map Option.get
      Expect.sequenceEqual messages receivedMessages "All messages received in order."
  ]
