namespace synthings.core

type Signal = interface end
type SignalValue = interface end
type Change = interface end
type TopicIdentifier = interface end
type BehaviorIdentifier = interface end
type Parameters = interface end

type EmptySignalValue =
    | EmptySignalValue
    interface SignalValue

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
