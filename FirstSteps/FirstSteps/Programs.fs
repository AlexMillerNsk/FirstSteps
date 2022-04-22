module FirstSteps.Core.Program

open System
open Elmish
open Elmish.WPF

type Todo = { Id: int; Value: string }
//Messages
type Msg =
    | SetState of string
    | AddTodo
    | RemoveToDo of obj
    | Plus
    | Minus
    | SetCount of int
//Models
type Model =
    { Count: int
      State: string
      ToDo: Todo list
    }

//Init
let init () =
    { State = String.Empty
      ToDo = []
      Count = 0},[]
//AddThings
let AddThings m =
    { m with
        ToDo = { Id = Random().Next(0,999); Value = m.State }:: m.ToDo
        State = String.Empty}
//Update
let update msg m =
    match msg with
    | SetState v          -> { m with State = v },[]
    | AddTodo             -> AddThings m, []
    | RemoveToDo b        -> { m with ToDo= m.ToDo |> List.filter (fun z -> z.Id <> (b:?> int)) },[]
    | SetCount k          -> { m with Count = k}, []
    | Plus                -> { m with Count = m.Count+1},[]
    | Minus               -> { m with Count = m.Count-1},[]
//Bindings
let bindings(): Binding<Model, Msg> list =
    [
        "SetState"      |> Binding.twoWay ((fun m -> m.State), SetState)
        "ToDo"          |> Binding.subModelSeq ((fun m -> m.ToDo), (fun y -> y.Id), (fun () ->
        [ "RemoveTodo"  |> Binding.cmdParam RemoveToDo
          "Id"          |> Binding.oneWay (fun (_, s) -> s.Id)
          "Value"       |> Binding.oneWay (fun (_, s) -> s.Value) ]))
        "AddToDo"       |> Binding.cmdIf (AddTodo, (fun m -> not <| String.IsNullOrEmpty(m.State)))
        "SetCount"      |> Binding.twoWay ((fun m -> m.Count), SetCount)
        "Plus"          |> Binding.cmd Plus
        "Minus"         |> Binding.cmd Minus
    
    ]

let Run window =
    Program.mkProgramWpf
        init
        update
        bindings
    |> Program.startElmishLoop ElmConfig.Default window

//App
