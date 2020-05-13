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
open System
open Thoth.Json.Net

type Thread = { thread : int
              ; realm : Realm
              ; theme : int
              ; author : int
              ; authorName : string
              ; createdAt : DateTime
              ; updatedAt : DateTime
              ; name : string
              ; ``open`` : bool
              ; sticky : bool
              } with
  static member jsonEncoder(xx) =
    Encode.object
      [ "thread", Encode.int xx.thread
      ; "realm", Realm.jsonEncoder xx.realm
      ; "theme", Encode.int xx.theme
      ; "author", Encode.int xx.author
      ; "authorName", Encode.string xx.authorName
      ; "createdAt", Encode.datetime xx.createdAt
      ; "updatedAt", Encode.datetime xx.updatedAt
      ; "name", Encode.string xx.name
      ; "open", Encode.bool xx.``open``
      ; "sticky", Encode.bool xx.sticky
      ]
end

namespace Api.Domain
open System
open Api

type Thread = { thread : int
              ; realm : Realm
              ; theme : int
              ; author : int
              ; authorName : string
              ; createdAt : DateTime
              ; updatedAt : DateTime
              ; name : string
              ; ``open`` : bool
              ; sticky : bool
              } with
  static member dto_() =
    ( ( fun (dto:Dto.Thread) ->
          match dto.realm |> fst (Realm.dto_()) with
          | None       -> None
          | Some realm -> Some { thread=dto.thread
                               ; realm=realm
                               ; theme=dto.theme
                               ; author=dto.author
                               ; authorName=dto.authorName
                               ; createdAt=dto.createdAt
                               ; updatedAt=dto.createdAt
                               ; name=dto.name
                               ; ``open``=dto.``open``
                               ; sticky=dto.sticky
                               }
      )
    , ( fun t -> { Dto.Thread.thread=t.thread
                 ; Dto.Thread.realm=t.realm |> snd (Realm.dto_())
                 ; Dto.Thread.theme=t.theme
                 ; Dto.Thread.author=t.author
                 ; Dto.Thread.authorName=t.authorName
                 ; Dto.Thread.createdAt=t.createdAt
                 ; Dto.Thread.updatedAt=t.createdAt
                 ; Dto.Thread.name=t.name
                 ; Dto.Thread.``open``=t.``open``
                 ; Dto.Thread.sticky=t.sticky
                 }
      )
    )
end
