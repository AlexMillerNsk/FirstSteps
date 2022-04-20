module FirstSteps.Core.Program

open System
open Elmish
open Elmish.WPF

//Messages
type Msg =
    | SetState
    | SetMinus
    | SetPlus
//Models
type Model =
    { State: int
      Input: string
    }
//Init
let init () =
    { State = 0
      Input = String.Empty },[]

//Update
let update msg m =
    match msg with
    | SetState                 -> { m with State = m.State },[]
    | SetMinus                 -> { m with State = m.State - 1 },[]
    | SetPlus                  -> { m with State = m.State + 1 },[]
//Bindings
let bindings(): Binding<Model, Msg> list =
    [
        "SetState "     |> Binding.oneWay(fun m -> m.State)
        "SetMinus "     |> Binding.cmd SetMinus
        "SetPlus "      |> Binding.cmd SetPlus
       
    ]

let Run window =
    Program.mkProgramWpf
        init
        update
        bindings
    |> Program.startElmishLoop ElmConfig.Default window

//App
