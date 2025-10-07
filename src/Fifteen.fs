module Fifteen

open System
open Feliz

let rnd = Random()

let computeRow index =
    int(Math.Floor(float(index - 1) / 4.0)), (index - 1) % 4

let computeBounds row col =
    [
        row - 1, col
        row + 1, col
        row, col - 1
        row, col + 1
    ]  |> List.filter (fun (r,c) -> r >= 0 && r < 4 && c >= 0 && c < 4)

let freeSlot = rnd.Next(1, 16)

[<ReactComponent>]
let Tile number =
    Html.div [
        prop.className "flex bg-amber-200 items-center justify-center h-24 w-24 box-border rounded-xl font-bold text-4xl select-none"
        prop.text (string number)
    ]

let generateTiles = 
    [1 .. 15] 
    |> List.sortBy (fun _ -> rnd.Next())  


let mutable index = 0

    
[<ReactComponent(true)>]
let Game () = 
    let emptySlot, setEmptySlot = React.useState (rnd.Next(1, 16))
    
    Html.div [
        prop.className "flex h-screen w-full items-center justify-center"
        prop.children [
            Html.div [
                prop.className "bg-[rgba(0,119,24,0.29)] flex p-6 rounded-lg"
                prop.children [ 
                    Html.div [
                        prop.className "grid grid-cols-4 gap-2"
                        prop.children [
                            index <- 0
                            let freeRow, freeCol = computeRow emptySlot
                            let freeBounds = computeBounds freeRow freeCol
                            printfn "free bounds: %A" freeBounds
                            for i in 1 .. 16 do
                                let row, col = computeRow i
                                let bounds = computeBounds row col 
                                let isClickable = List.exists (fun (r,c) ->  r = row && c = col) freeBounds  

                                Html.div [
                                    prop.className "bg-teal-800 flex rounded-xl h-24 w-24 box-border"
                                    if isClickable then
                                        prop.className "cursor-pointer"
                                        prop.onClick (fun _ -> 
                                            // printfn "Clicked %A" bounds
                                            printfn "Clicked %i" i
                                            setEmptySlot i
                                        )
                                    // prop.text (sprintf "%i = (%i, %i)" i row col)
                                    prop.children [
                                        if i <> emptySlot then
                                            Tile generateTiles.[index] 
                                            index <- index + 1
                                    ]
                                ] 
                        ]
                    ]
                ]
            ]
        ]
    ]