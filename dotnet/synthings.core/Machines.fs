namespace synthings.core
open System

module Machines =
    open Signals
    open Workers
    
    type Machine =
        {
            Id : Guid;
            Name : string;
            Worker : Worker;
            Input : IObserver<Signal>;
            Output : IObservable<Signal>;
        }
    
    let connect (upstream : Machine) (downstream : Machine) = upstream.Output.Subscribe downstream.Input
    
    let drive (machine : Machine) (signal : Signal) = machine.Input.OnNext signal
    
    let createMachine (name : string) (worker : Worker) =
        let output = Reactive.createObservable
        let receiver (signal : Signal) =
            let result = worker signal
            match result with
            | Some result -> output.OnNext result
            | None -> ()
        let input = Reactive.createObserver receiver
        { Id = Guid.NewGuid(); Name = name; Worker = worker; Input = input; Output = output}