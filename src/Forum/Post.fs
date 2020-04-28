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

module HfVault.Forum.Post
#nowarn "62"
#light "off"

open System
open Aether.Operators
open HfVault

type T = { realm : Realm.T
         ; id : int
         ; name : string
         ; createdAt : DateTime
         ; content : string
         }

let new_ = { realm=Unchecked.defaultof<_>
           ; id=0
           ; name=null
           ; createdAt=Unchecked.defaultof<_>
           ; content=null
           }

let realm_ = (fun {realm=r} -> r), (fun r t -> {t with realm=r})
let id_ = (fun {id=i} -> i), (fun i t -> {t with id=i})
let name_ = (fun {name=n} -> n), (fun n t -> {t with name=n})
let createdAt_ = (fun {createdAt=c} -> c), (fun c t -> {t with createdAt=c})
let content_ = (fun {content=c} -> c), (fun c t -> {t with content=c})

let makeNewHfUser xx =
  let author =
    Domain.Author.new_
    |> xx.name^=Domain.Author.name_
  in
  Domain.HfUser.new_
  |> author^=Domain.HfUser.author_
  |> xx.id^=Domain.HfUser.hfid_
  |> xx.realm^=Domain.HfUser.realm_
