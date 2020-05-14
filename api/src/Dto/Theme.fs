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
             ; threads : int64
             } with
  static member jsonEncoder(xx) =
    Encode.object
      [ "name", Encode.string xx.name
      ; "hfid", Encode.int xx.hfid
      ; "realm", Realm.jsonEncoder xx.realm
      ; "threads", Encode.int (int xx.threads)
      ]
end

namespace Api.Domain
open Api

type Theme = T of (string * int * Realm * int64) with
  static member dto_() =
    ( ( fun (dto:Dto.Theme) ->
          match dto.realm |> fst (Realm.dto_()) with
          | Some realm -> Some (T (dto.name, dto.hfid, realm, dto.threads))
          | None       -> None
      )
    , ( fun (T (name, hfid, realm, threads)) ->
          { Dto.Theme.name=name
          ; Dto.Theme.hfid=hfid
          ; Dto.Theme.realm=realm |> snd (Realm.dto_())
          ; Dto.Theme.threads=threads
          }
      )
    )
end
