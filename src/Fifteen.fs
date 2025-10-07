module Fifteen

open System
open Feliz

let rnd = Random()

[<ReactComponent>]
let Tile number clickable =

    Html.div [
        prop.className "flex bg-amber-200 items-center justify-center h-24 w-24 box-border rounded-xl text-3xl select-none"
        prop.style [
            if clickable then
                style.cursor.pointer
        ]
        prop.text (string number)
    ]

let computeRow index =
    int(Math.Floor(float(index - 1) / 4.0)), (index - 1) % 4

let freeSlot = rnd.Next(1, 16)

[<ReactComponent(true)>]
let Game () = 
    
    Html.div [
        prop.className "flex h-screen w-full items-center justify-center"
        prop.children [
            Html.div [
                prop.className "bg-[rgba(0,119,24,0.29)] flex p-6 rounded-lg"
                prop.children [ 
                    Html.div [
                        prop.className "grid grid-cols-4 gap-2"
                        prop.children [
                            for i in 1 .. 16 do
                                Html.div [
                                    prop.className "bg-teal-800 flex rounded-xl h-24 w-24 box-border"
                                    // prop.text (computeRow i |> sprintf "%A" )
                                    prop.children [
                                        if i <> freeSlot then
                                            Tile (computeRow i) true
                                    ]
                                ] 
                        ]
                    ]
                ]
            ]
        ]
    ]