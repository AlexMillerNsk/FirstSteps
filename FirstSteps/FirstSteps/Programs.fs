module FirstSteps.Core.Program

open System
open Elmish
open Elmish.WPF

//Messages
type Msg =
    | SetState of string
    | SetMinus
    | SetPlus
    | AddTodo
//Models
type Model =
    { State: string
      Input: string
      ToDo: string list
    }
//Init
let init () =
    { State = String.Empty
      Input = String.Empty 
      ToDo = List.Empty},[]
//AddThings
let AddThings m = 
     { m with
        ToDo =  m.State :: m.ToDo}
//Update
let update msg m =
    match msg with
    | SetState v               -> { m with State = v },[]
    | SetMinus                 -> AddThings m, []
    | SetPlus                  -> { m with State = m.State },[]
    | AddTodo                  -> AddThings m, []
//Bindings
let bindings(): Binding<Model, Msg> list =
    [
        "SetState "     |> Binding.oneWay(fun m -> m.State)
        "SetMinus "     |> Binding.cmd SetMinus
        "SetPlus "      |> Binding.cmd SetPlus
        "ToDo"          |> Binding.oneWay(fun m -> m.ToDo)
        "AddToDo"       |> Binding.cmd AddTodo
    ]

let Run window =
    Program.mkProgramWpf
        init
        update
        bindings
    |> Program.startElmishLoop ElmConfig.Default window

//App
