namespace synthings.core

type Change = interface end
type TopicIdentifier = interface end
type BehaviorIdentifier = interface end

type TopicDescriptor =
    {
        Topic : TopicIdentifier;
        DisplayName : string;
        Id : Identifier
    }

type BehaviorDescriptor =
    {
        Behavior : BehaviorIdentifier;
        DisplayName : string;
        Id : Identifier
    }

type LibraryResolver =
    abstract member Origin : string
    abstract member Name : string
    abstract member listTopics : unit -> TopicDescriptor list
    abstract member listBehaviors : TopicDescriptor -> BehaviorDescriptor list
    abstract member createMachine : BehaviorDescriptor -> Machine
