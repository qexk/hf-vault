(*
    Copyright (C) 2020  Contributors as noted in the AUTHORS.md file
    This file is part of hf-vault, an Eternal Twin preservation project.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*)

module HfVault.__main__
#nowarn "62"
#light "off"

open Aether
open Aether.Operators
open HtmlAgilityPack
open HfVault
open HfVault.Optics

type Options = { realm : Realm.T
               } with
  static member new_ = { realm=Realm.FR
                       }

  static member usage = """Usage:
  dotnet hf-vault.exe [options]

Options:
  -r REALM, --realm REALM  Set the realm to scrape.
                           [type: FR | EN | ES, default: FR]
  """
end

let rec parseArgv o = function
| ("-r"|"--realm")::r::tl ->
    begin match r^.(Prism.ofEpimorphism String.realm_) with
    | Some r -> parseArgv {o with realm=r} tl
    | None   -> Error (r + ": invalid realm")
    end
| jaj::_ -> Error (jaj + ": unknown option")
| []     -> Ok o

[<EntryPoint>]
let main argv =
  match parseArgv Options.new_ (Array.toList argv) with
  | Error e -> Printf.eprintfn "error: %s" e; -1
  | Ok o    -> 0
