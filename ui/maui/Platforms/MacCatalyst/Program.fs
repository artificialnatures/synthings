namespace synthings.ui.maui

open UIKit
(*
module Program =
    [<EntryPoint>]
    let main args =
        UIApplication.Main(args, null, typeof<AppDelegate>)
        0
*)

module MauiApplication =
    let launch () =
        let args : System.String array = Array.empty
        UIApplication.Main(args, null, typeof<AppDelegate>)
