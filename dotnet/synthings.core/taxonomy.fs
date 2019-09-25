namespace synthings.core

type TopicIdentifier = interface end

type BehaviorIdentifier = interface end

type Parameters = interface end

type TopicDescriptor =
    {
        Topic : TopicIdentifier;
        DisplayName : string;
        Id : System.Guid
    }

type BehaviorDescriptor =
    {
        Behavior : BehaviorIdentifier;
        DisplayName : string;
        Id : System.Guid
    }

type LibraryResolver =
    abstract member Origin : string
    abstract member Name : string
    abstract member listTopics : unit -> TopicDescriptor list
    abstract member listBehaviors : TopicDescriptor -> BehaviorDescriptor list
    abstract member createMachine : BehaviorDescriptor -> Machine
