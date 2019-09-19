namespace synthings.core

type Identifier =
    {
        Name : string;
        Id : System.Guid
    }

type MachineBuilder = System.Guid -> Machine

type Topic =
    {
        Identifier : Identifier;
        BuildMachine : MachineBuilder;
        Behaviors : Identifier list
    }

type Library =
    {
        Topics : Topic list;
    }

module library =
    type CoreTopic =
        | Basic
            | Relay
            | Error
    
    let internal identifiers =
        Map.empty
        |> Map.add CoreTopic.Basic {Id = System.Guid.Parse("03B71882-8158-40F9-8CC7-71347E3FC784"); Name = CoreTopic.Basic.ToString()}
        |> Map.add CoreTopic.Relay {Id = System.Guid.Parse("DCB280FA-ABFF-4564-A84D-14D38D9405E3"); Name = CoreTopic.Relay.ToString()}
        |> Map.add CoreTopic.Error {Id = System.Guid.Parse("7D956605-9C75-4482-9B6E-C0620A69FCFC"); Name = CoreTopic.Error.ToString()}
    
    let internal reverseLookup =
        Map.empty
        |> Map.add (identifiers.Item CoreTopic.Basic).Id CoreTopic.Basic
        |> Map.add (identifiers.Item CoreTopic.Relay).Id CoreTopic.Relay
        |> Map.add (identifiers.Item CoreTopic.Error).Id CoreTopic.Error
    
    let internal behaviors =
        Map.empty
        |> Map.add (identifiers.Item CoreTopic.Relay).Id behavior.relay
        |> Map.add (identifiers.Item CoreTopic.Error).Id behavior.error
    
    let internal mapBehavior (behaviorId : System.Guid) =
        let identifier = reverseLookup.TryFind behaviorId
        match identifier with
        | Some identifier -> machine.createMachine (identifier.ToString()) (behaviors.Item behaviorId)
        | None -> machine.createMachine (Error.ToString()) (behaviors.Item (identifiers.Item CoreTopic.Error).Id)
    
    let internal mapTopic (topicId : System.Guid) =
        let identifier = reverseLookup.TryFind topicId
        match identifier with
        | Some identifier -> mapBehavior
        | None -> mapBehavior
    
    let internal basicTopic =
        {
            Identifier = identifiers.Item Basic;
            BuildMachine = mapTopic (identifiers.Item Basic).Id;
            Behaviors = [identifiers.Item Relay; identifiers.Item Error]
        }
    
    let create () = {Topics = [basicTopic]}
