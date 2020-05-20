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

module Api.__main__
#nowarn "62"
#light "off"

open KestrelInterop

[<EntryPoint>]
let main _ =
  let configureApp = ApplicationBuilder.useFreya Router.root in
  WebHost.create ()
  |> WebHost.bindTo [|"http://localhost:5000"|]
  |> WebHost.configure configureApp
  |> WebHost.buildAndRun;
  0
