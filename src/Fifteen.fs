module Fifteen

open System
open Feliz

type Position = {X: int; Y:int}

type Slot = Position * string

type AppState = { Slots: Slot list; FreeSlot: Position; Moves: int }

let rnd = Random()

let computeBounds row col =
    [
        row - 1, col
        row + 1, col
        row, col - 1
        row, col + 1
    ]  |> List.filter (fun (r,c) -> r >= 0 && r < 4 && c >= 0 && c < 4)

let freeSlot = rnd.Next(1, 16)

[<ReactComponent>]
let Tile number isClickable =
    Html.div [
        prop.className "flex bg-amber-200 items-center justify-center h-24 w-24 box-border rounded-xl font-bold text-4xl select-none drop-shadow-sm"
        prop.text (string number)
    ]

[<ReactComponent>]
let MovesPanel moves =
    Html.div [
        prop.className "flex flex-col items-center font-bold bg-blue-200 px-6 py-1 rounded-lg"
        prop.children [
            Html.div [
                prop.className "uppercase"
                prop.text "Moves"
            ]
            Html.div [
                prop.className "text-2xl opacity-75"
                prop.text (sprintf "%i" moves)
            ]
        ]
    ]  

let initialState  =
    let tags = [1 .. 16] |> List.sortBy (fun _ -> rnd.Next()) 
    [ 
        for x in 0 .. 3 do 
        for y in 0 .. 3 do
        yield { X = x; Y = y}
    ]
    |> List.mapi (fun i pos -> pos, string(List.item i tags) )
    |> fun slots ->
        let pos, _ = Seq.find (fun (_, tag) -> tag = "16") slots
        { Slots = slots; FreeSlot = pos; Moves = 0}

let slotSelected (oldState : AppState) (selectedSlot: Slot) =
    let oldP, OldT = selectedSlot
    let newSlot = 
        oldState.Slots 
        |> List.map (fun s -> 
            let p, t = s 
            if p.X = oldState.FreeSlot.X && p.Y = oldState.FreeSlot.Y then p, OldT
            else s
        ) 
    {oldState with 
        FreeSlot = oldP
        Slots = newSlot
        Moves = oldState.Moves + 1
        }

    
[<ReactComponent(true)>]
let Game () =  
    let appState, setAppState = React.useStateWithUpdater initialState
    
    Html.div [
        prop.className "flex flex-col h-screen w-full items-center justify-center"
        prop.children [
            MovesPanel appState.Moves
            Html.div [
                prop.className "bg-[rgba(0,119,24,0.29)] flex p-6 rounded-lg m-4"
                prop.children [  
                    Html.div [
                        prop.className "grid grid-cols-4 gap-2"
                        prop.children [
                            let clickableSlots = computeBounds appState.FreeSlot.X appState.FreeSlot.Y

                            for slot in appState.Slots do
                                let pos, title = slot
                                let isClickable = List.exists (fun (r,c) ->  r = pos.X && c = pos.Y) clickableSlots  
                                Html.div [
                                    prop.className "bg-teal-800 flex rounded-xl h-24 w-24 box-border"
                                    if isClickable then
                                        prop.className "cursor-pointer"
                                        prop.onClick (fun _ ->  
                                            setAppState(fun prev -> 
                                                slotSelected prev slot)
                                        )
                                    prop.children [ 
                                        if appState.FreeSlot <> pos then
                                            Tile title isClickable
                                    ]
                                ] 
                                
                        ]
                    ]
                ]
            ]
        ]
    ]