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
open HfVault

type T = { realm : Realm.T
         ; author : int
         ; createdAt : DateTime
         ; content : string
         }

let new_ = { realm=Unchecked.defaultof<_>
           ; author=0
           ; createdAt=Unchecked.defaultof<_>
           ; content=null
           }

let realm_ = (fun {realm=r} -> r), (fun r t -> {t with realm=r})
let author_ = (fun {author=a} -> a), (fun a t -> {t with author=a})
let createdAt_ = (fun {createdAt=c} -> c), (fun c t -> {t with createdAt=c})
let content_ = (fun {content=c} -> c), (fun c t -> {t with content=c})
