module FirstSteps.Core.Program

open System
open Elmish
open Elmish.WPF
open Types

//Messages
type Msg =
    | Rendered
    | SetInput of string
    | SetValue of int * int * float
//Models
type Model =
    { State: string
      Input: string
      Items: Item list
      Total: float }
//Init
let init () =
    { State = String.Empty
      Input = String.Empty
      Items = []
      Total = 0.0 }, []

//Constructor
[<RequireQualifiedAccess>]
module ItemConstructor =
    let Create name value1 value2 sum: Item =
        { Id = Random().Next(1000, 9999)
          Name = name
          Value1 = value1
          Value2 = value2
          Sum = sum }
    let Default() =
        { Id = Random().Next(1000, 9999)
          Name = "Default"
          Value1 = 0.0
          Value2 = 0.0
          Sum = 0.0 }

//UpdateUtils
let GetItems() = [0..100] |> List.map(fun _ -> ItemConstructor.Default())

let SetValue model valueIndex id value =

    let setValue item =
        match valueIndex with
        | 1 -> { item with Value1 = value }
        | 2 -> { item with Value2 = value }
        | _ -> item
        
    let map item = if item.Id = id then setValue item else item
    let model = { model with Items = List.map map model.Items }
    let sum item = { item with Sum = item.Value2 + (model.Items |> Seq.sumBy(fun x -> x.Value1)) }
    let model = { model with Items = model.Items |> List.map sum }
    { model with Total = model.Items |> Seq.sumBy(fun x -> x.Sum) }
//Commands
let CalculateTotalCmd (m: Model) = fun dispatch ->
    async {
        let action() = dispatch <| SetInput "Some string"
        
        
        ()
    } |> Async.StartImmediate
//Update
let update msg m =
//Logger.Debug($"msg: {msg}")
    match msg with
    | Rendered                 -> { m with State = "Hello!"; Items = GetItems() }, Cmd.none
    | SetInput v               -> { m with Input = v }, Cmd.none
    | SetValue (vn, id, v)     -> SetValue m vn id v , [ CalculateTotalCmd m]
//Bindings
let bindings(): Binding<Model, Msg> list =
    [
        "Rendered"  |> Binding.cmd Rendered
        "State"     |> Binding.oneWay(fun m -> m.State)
        "Input"     |> Binding.twoWay((fun m -> m.Input), SetInput)
        "Total"     |> Binding.oneWay(fun m -> m.Total)
        "Items"     |> Binding.subModelSeq((fun m -> m.Items), (fun s -> s.Id), (fun () -> [
            "Name"   |> Binding.oneWay (fun (_, e) -> e.Name)
            "Value1" |> Binding.twoWay ((fun (_, e) -> e.Value1), (fun v (_, e) -> SetValue (1, e.Id, v)))
            "Value2" |> Binding.twoWay ((fun (_, e) -> e.Value2), (fun v (_, e) -> SetValue (2, e.Id, v)))
            "Sum"    |> Binding.oneWay (fun (_, e) -> e.Sum) ] ))
    ]

//App
let Run window =
    Program.mkProgramWpf
        init
        update
        bindings
    |> Program.startElmishLoop ElmConfig.Default window