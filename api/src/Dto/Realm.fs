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

namespace Api
#nowarn "62"
#light "off"

namespace Api.Dto
open Thoth.Json.Net
open Api

type Realm = {value : DbTypes.Realm} with
  static member ofString = fun (s:string) ->
    match s.ToUpperInvariant() with
    | "FR" -> Some {value=DbTypes.Realm.FR}
    | "EN" -> Some {value=DbTypes.Realm.EN}
    | "ES" -> Some {value=DbTypes.Realm.ES}
    | _    -> None

  static member toString = fun xx -> string xx.value

  static member jsonEncoder = fun xx ->
    Encode.object ["realm", Encode.string (string xx.value)]
end

namespace Api.Domain
open Api

type Realm = FR
           | EN
           | ES with
  static member dto_ =
    ( ( fun (dto:Dto.Realm) ->
          match dto.value with
          | DbTypes.Realm.FR -> Some FR
          | DbTypes.Realm.EN -> Some EN
          | DbTypes.Realm.ES -> Some ES
          | _                -> None
      )
    , function
      | FR -> {Dto.Realm.value=DbTypes.Realm.FR}
      | EN -> {Dto.Realm.value=DbTypes.Realm.EN}
      | ES -> {Dto.Realm.value=DbTypes.Realm.ES}
    )
end
