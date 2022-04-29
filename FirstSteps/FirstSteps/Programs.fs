module FirstSteps.Core.Program

open System
open Elmish
open Elmish.WPF

type Todo = { Id: int; Value: string; ToDoText: string }
//Messages
type Msg =
    | SetState of string
    | AddTodo
    | RemoveToDo of obj
    | Plus
    | Minus
    | SetCount of int
    | SetToDos of Todo list
    | SetToDoText of string * int
    | UpdateToDo of string * int
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
        ToDo = { Id = Random().Next(0,999); Value = m.State; ToDoText = String.Empty }:: m.ToDo
        State = String.Empty}

let SetToDoText (m: Model) value id = 
    let todos = m.ToDo
    let maper t = if t.Id = id then { t with ToDoText = value } else t
    let todos = List.map maper todos
    { m with ToDo = todos }

let updateToDo (m: Model) value id =
    let todos= m.ToDo
    let updater t = if t.Id = id then { t with Value = value } else t
    let todos = List.map updater todos
    { m with ToDo = todos }


//Update
let update msg m =
    match msg with
    | SetState v          -> { m with State = v },[]
    | AddTodo             -> AddThings m, []
    | RemoveToDo b        -> { m with ToDo= m.ToDo |> List.filter (fun z -> z.Id <> (b:?> int)) },[]
    | SetCount k          -> { m with Count = k}, []
    | Plus                -> { m with Count = m.Count+1},[]
    | Minus               -> { m with Count = m.Count-1},[]
    | SetToDos t          -> { m with ToDo = t }, []
    | SetToDoText (v, id) -> SetToDoText m v id, []
    | UpdateToDo (v, id)  -> updateToDo m v id, []
//Bindings
let setToDoText a (m, s) =  Msg.SetToDoText (a, s.Id)
let ToDoBinding() =
    [ "RemoveTodo"  |> Binding.cmdParam RemoveToDo
      "Id"          |> Binding.oneWay (fun (_, s) -> s.Id)
      "Value"       |> Binding.oneWay (fun (_, s) -> s.Value)
      "ToDoText"    |> Binding.twoWay ((fun (_, s) -> s.ToDoText),setToDoText)
      "UpdateTodo"  |> Binding.cmdIf ((fun (_, s) -> UpdateToDo(s.ToDoText, s.Id)),(fun (m, s) -> not <| String.IsNullOrEmpty(s.ToDoText)))  
    ]

let bindings(): Binding<Model, Msg> list =
    [
        "SetState"      |> Binding.twoWay ((fun m -> m.State), SetState)
        "ToDo"          |> Binding.subModelSeq ((fun m -> m.ToDo), (fun y -> y.Id), ToDoBinding)   
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
