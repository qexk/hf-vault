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

module Api.Machine.Pervasives
#nowarn "62"
#light "off"

open Freya.Core
open Freya.Core.Operators
open Freya.Routers.Uri.Template
open Freya.Types.Http
open Api

type MediaType with
  static member HalJson =
    MediaType (Type "application", SubType "hal+json", Parameters Map.empty)
end

let realm_ = Route.atom_ "realm"

let realm = freya
{ let inline ( >>= ) v f = Option.bind f v in
  let! realm = !.realm_ in
  return realm
     >>= Dto.Realm.ofString
     >>= (fst Domain.Realm.dto_)
} |> Freya.memo
