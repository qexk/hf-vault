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

namespace HfVault
#nowarn "62"
#light "off"

namespace HfVault.Dto
module HfPost = begin
  open Aether.Operators
  open HfVault

  type T = T of int * Realm.T * Post.T

  type DbRepr = { hfid : int
                ; realm : DbTypes.Realm
                ; post : Post.DbRepr
                }

  let toDb (T (hfid, realm, post)) =
     { hfid=hfid
     ; realm=realm^.Realm.dbRealm_
     ; post=post |> Post.toDb
     }
end

namespace HfVault.Domain
module HfPost = begin
  open HfVault

  type T = { hfid : int
           ; realm : Realm.T
           ; post : Post.T
           }

  let new_ = {hfid=0; realm=Unchecked.defaultof<_>; post=Post.new_}
  let hfid_ = (fun {hfid=h} -> h), (fun h t -> {t with hfid=h})
  let realm_ = (fun {realm=r} -> r), (fun r t -> {t with realm=r})
  let post_ = (fun {post=p} -> p), (fun p t -> {t with post=p})

  let dto_ =
  ( ( fun (Dto.HfPost.T (hfid, realm, post)) ->
        let post = post |> (fst Post.dto_) in
        {hfid=hfid; realm=realm; post=post}
    )
  , ( fun t ->
        let post = t.post |> (snd Post.dto_) in
        Dto.HfPost.T (t.hfid, t.realm, post)
    )
  )
end
