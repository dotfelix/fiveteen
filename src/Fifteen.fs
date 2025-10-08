module Fifteen

open System
open Feliz

type Position = { X: int; Y: int }

type Slot = Position * string

type AppState =
    { Slots: Slot list
      FreeSlot: Position
      Moves: int }

let rnd = Random()

let computeBounds row col =
    [ row - 1, col; row + 1, col; row, col - 1; row, col + 1 ]
    |> List.filter (fun (r, c) -> r >= 0 && r < 4 && c >= 0 && c < 4)

[<ReactComponent>]
let Tile number =
    Html.div [ prop.className "tile"; prop.text (string number) ]

[<ReactComponent>]
let MovesPanel moves =
    Html.div
        [ prop.className "moves"
          prop.children
              [ Html.div [ prop.className "uppercase"; prop.text "Moves" ]
                Html.div [ prop.className "text-2xl opacity-75"; prop.text (sprintf "%i" moves) ] ] ]

[<ReactComponent>]
let Title (title: string) =
    Html.div [ prop.className "title"; prop.text title ]

let initialState =
    let tags = [ 1..16 ] |> List.sortBy (fun _ -> rnd.Next())

    [ for x in 0..3 do
          for y in 0..3 do
              yield { X = x; Y = y } ]
    |> List.mapi (fun i pos -> pos, string (List.item i tags))
    |> fun slots ->
        let pos, _ = Seq.find (fun (_, tag) -> tag = "16") slots

        { Slots = slots
          FreeSlot = pos
          Moves = 0 }

let slotSelected (oldState: AppState) (selectedSlot: Slot) =
    let oldP, OldT = selectedSlot

    let newSlot =
        oldState.Slots
        |> List.map (fun s ->
            let p, t = s

            if p.X = oldState.FreeSlot.X && p.Y = oldState.FreeSlot.Y then
                p, OldT
            else
                s)

    { oldState with
        FreeSlot = oldP
        Slots = newSlot
        Moves = oldState.Moves + 1 }


[<ReactComponent>]
let Reset setAppState =
    Html.div
        [ prop.className "reset"
          prop.onClick (fun _ -> setAppState (fun _ -> initialState))
          prop.text "Reset" ]

[<ReactComponent>]
let Instruction keys =
    Html.div
        [ prop.className "text-wrap w-120 p-4"
          prop.children
              [ Html.h1 [ 
                        prop.className "font-bold text-3xl uppercase" 
                        prop.text "Instruction:" ]
                Html.p
                    [ 
                        prop.className "font-medium text-2xl"
                        prop.text """Move the tiles in the grid to arrange them in numerical order, 
                                    from 1 to 15. To move a tile, click on it; only tiles immediately 
                                    adjacent to the empty space can be moved..""" ] ] ]

[<ReactComponent(true)>]
let Game () =
    let appState, setAppState = React.useStateWithUpdater initialState

    Html.div
        [ prop.className "flex flex-col h-screen w-full items-center justify-center"
          prop.children
              [ Html.div
                    [ prop.className "flex gap-6"
                      prop.children
                          [ Html.div
                                [ prop.className "flex flex-col items-center gap-4"
                                  prop.children [ MovesPanel appState.Moves; Reset setAppState ] ]
                            Title "Fifteen Puzzle Game" ] ]
                Html.div
                    [ prop.className "bg-[rgba(0,119,24,0.29)] flex p-6 rounded-lg m-4"
                      prop.children
                          [ Html.div
                                [ prop.className "grid grid-cols-4 gap-2"
                                  prop.children
                                      [ let clickableSlots = computeBounds appState.FreeSlot.X appState.FreeSlot.Y

                                        for slot in appState.Slots do
                                            let pos, title = slot

                                            let isClickable =
                                                List.exists (fun (r, c) -> r = pos.X && c = pos.Y) clickableSlots

                                            Html.div
                                                [ prop.className "bg-teal-800 flex rounded-xl h-24 w-24 box-border"
                                                  if isClickable then
                                                      prop.className "cursor-pointer"

                                                      prop.onClick (fun _ ->
                                                          setAppState (fun prev -> slotSelected prev slot))
                                                  prop.children
                                                      [ if appState.FreeSlot <> pos then
                                                            Tile title ] ]

                                        ] ] ] ]
                Instruction "" ] ]
