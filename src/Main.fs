module Main

open Feliz
open Browser.Dom
open Fable.Core.JsInterop

importSideEffects "./index.css"

let root = ReactDOM.createRoot(document.getElementById "feliz-app")
root.render(Fifteen.Game())