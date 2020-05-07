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

type Theme = { name : string
             ; hfid : int
             ; realm : Realm
             } with
  static member jsonEncoder = fun xx ->
    Encode.object
      [ "name", Encode.string xx.name
      ; "hfid", Encode.int xx.hfid
      ; "realm", Realm.jsonEncoder xx.realm
      ]
end

namespace Api.Domain
open Api

type Theme = T of (string * int * Realm) with
  static member dto_ =
    ( ( fun (dto:Dto.Theme) ->
          match dto.realm |> fst Realm.dto_ with
          | Some realm -> Some (T (dto.name, dto.hfid, realm))
          | None       -> None
      )
    , ( fun (T (name, hfid, realm)) ->
          { Dto.Theme.name=name
          ; Dto.Theme.hfid=hfid
          ; Dto.Theme.realm=realm |> snd Realm.dto_
          }
      )
    )

  static member name_ =
    ( (fun (T (name, _, _)) -> name)
    , (fun name (T (_, hfid, realm)) -> T (name, hfid, realm))
    )

  static member hfid_ =
    ( (fun (T (_, hfid, _)) -> hfid)
    , (fun hfid (T (name, _, realm)) -> T (name, hfid, realm))
    )

  static member realm_ =
    ( (fun (T (_, _, realm)) -> realm)
    , (fun realm (T (name, hfid, _)) -> T (name, hfid, realm))
    )
end
