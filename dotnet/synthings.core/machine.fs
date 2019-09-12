namespace synthings.core

module machine =
    open System
    open signal
    open behavior
    
    type Machine =
        {
            Id : Guid;
            Name : string;
            Behavior : Behavior;
            Input : IObserver<Signal>;
            Output : IObservable<Signal>;
        }