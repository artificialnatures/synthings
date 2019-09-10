namespace synthings.core

module Reactive =
    open System.Reactive
    open System.Reactive.Subjects
    open Signals
    
    //TODO: handle onError, onComplete
    let createObserver (onNext : Signal -> unit) = Observer.Create (fun signal -> onNext signal)
    let createObservable = new Subject<Signal>()