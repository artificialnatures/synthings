namespace synthings.core

module Relay =
    open Signals
    let worker (signal : Signal) = Some signal
    let createMachine (name : string) = Machines.createMachine name worker
    