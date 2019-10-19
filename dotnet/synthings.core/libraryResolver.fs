namespace synthings.core

type LibraryResolver =
    abstract member Origin : string
    abstract member Name : string
    abstract member listTopics : unit -> TopicDescriptor list
    abstract member listBehaviors : TopicDescriptor -> BehaviorDescriptor list
    abstract member createMachine : BehaviorDescriptor -> Machine
