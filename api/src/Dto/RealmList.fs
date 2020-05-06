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

type RealmList = {realms : Realm list} with
  static member jsonEncoder = fun xx ->
    xx.realms |> List.map Realm.jsonEncoder |> Encode.list
end

namespace Api.Domain
open Api

type RealmList = {realms : Realm list} with
  static member dto_ =
    ( ( fun (dto:Dto.RealmList) ->
          { realms=dto.realms
                |> List.map (fst Realm.dto_)
                |> List.choose id
          }
      )
    , (fun xx -> {Dto.RealmList.realms=List.map (snd Realm.dto_) xx.realms})
    )
end
