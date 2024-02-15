namespace synthings.transmission

type MessagingImplementation =
    | ZeroMQ
    | Channels
type Message<'proposal> =
    {
        sender : Identifier
        proposal : 'proposal
    }
type MessageDispatcher<'proposal> = Identifier -> 'proposal -> unit //Identifier and 'proposal are combined to form a Message<'proposal>
type MessageReceiver<'proposal> = unit -> Message<'proposal> option

module Json =
    open System.Text.Json
    open System.Text.Json.Serialization
    let options = JsonFSharpOptions.Default().ToJsonSerializerOptions()
    JsonFSharpOptions.Default().AddToJsonSerializerOptions(options)

    let toJson message =
        JsonSerializer.Serialize(message, options)
    
    let fromJson<'message> (json : string) =
        JsonSerializer.Deserialize<'message>(json, options)

module MessageQueue =
    open System.Threading.Channels
    open NetMQ
    open NetMQ.Sockets
    
    let createChannel<'proposal> () : (MessageDispatcher<'proposal> * MessageReceiver<'proposal>) =
        let channel = Channel.CreateUnbounded<Message<'proposal>>()
        //send adds a message to the queue (the producer side)
        let send (sender : Identifier) (proposal : 'proposal) =
            channel.Writer.WriteAsync({sender=sender; proposal=proposal}).AsTask()
            |> Async.AwaitTask
            |> Async.RunSynchronously
        //receive removes a message from the queue (the consumer side)
        let receive () =
            if channel.Reader.Count > 0 then
                channel.Reader.ReadAsync().AsTask()
                |> Async.AwaitTask
                |> Async.RunSynchronously
                |> Some
            else
                None
        send, receive

    let createPublishSubscribe<'request, 'response> () : (MessageDispatcher<'request> * MessageReceiver<'request> * MessageDispatcher<'response> * MessageReceiver<'response>) =
        let channelName = "Default"

        let responseSocket = new ResponseSocket()
        responseSocket.Bind("tcp://*:5565")

        let requestSocket = new RequestSocket()
        requestSocket.Connect("tcp://127.0.0.1:5565")

        let publisherSocket = new PublisherSocket()
        publisherSocket.Bind("tcp://*:5566")

        let subscriberSocket = new SubscriberSocket()
        subscriberSocket.Connect("tcp://127.0.0.1:5566")
        subscriberSocket.Subscribe(channelName)

        let sendRequest (sender : Identifier) (request : 'request) =
            let message = {sender=sender; proposal=request}
            requestSocket.SendFrame(Json.toJson message)
            match requestSocket.ReceiveFrameString () with
            | "OK" -> ()
            | stringResponse ->
                printfn "Received %s response to request." stringResponse

        let receiveRequest () =
            if responseSocket.HasIn then
                let request =
                    responseSocket.ReceiveFrameString()
                    |> Json.fromJson<Message<'request>>
                responseSocket.SendFrame("OK")
                Some request
            else None

        let publishMessage (sender : Identifier) (response : 'response) =
            let message = {sender=sender; proposal=response}
            publisherSocket.SendMoreFrame(channelName).SendFrame(Json.toJson message)
        
        let receiveSubscriptionMessage () =
            if subscriberSocket.HasIn then
                let channel = subscriberSocket.ReceiveFrameString()
                let message = 
                    subscriberSocket.ReceiveFrameString()
                    |> Json.fromJson<Message<'response>>
                Some message
            else None
        sendRequest, receiveRequest, publishMessage, receiveSubscriptionMessage
    
    let create<'request, 'response> messagingImplementation =
        match messagingImplementation with
        | ZeroMQ ->
            let submitProposal, receiveProposal, _, _ =
                createPublishSubscribe<'request, 'response> ()
            (submitProposal, receiveProposal)
        | Channels ->
            let submitProposal, receiveProposal = createChannel<'request> ()
            //let submitRenderTask, receiveRenderTask = MessageQueue.create<'response> ()
            (submitProposal, receiveProposal)
