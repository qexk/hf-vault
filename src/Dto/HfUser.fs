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
module HfUser = begin
  open Aether.Operators
  open HfVault

  type T = T of int * Realm.T * Author.T

  type DbRepr = { hfid : int
                ; realm : DbTypes.Realm
                ; author : Author.DbRepr
                }

  let toDb (T (hfid, realm, author)) =
     { hfid=hfid
     ; realm=realm^.Realm.dbRealm_
     ; author=author |> Author.toDb
     }
end

namespace HfVault.Domain
module HfUser = begin
  open HfVault

  type T = { hfid : int
           ; realm : Realm.T
           ; author : Author.T
           }

  let new_ = {hfid=0; realm=Unchecked.defaultof<_>; author=Author.new_}
  let hfid_ = (fun {hfid=h} -> h), (fun h t -> {t with hfid=h})
  let realm_ = (fun {realm=r} -> r), (fun r t -> {t with realm=r})
  let author_ = (fun {author=th} -> th), (fun th t -> {t with author=th})

  let dto_ =
  ( ( fun (Dto.HfUser.T (hfid, realm, author)) ->
        let author = author |> (fst Author.dto_) in
        {hfid=hfid; realm=realm; author=author}
    )
  , ( fun t ->
        let author = t.author |> (snd Author.dto_) in
        Dto.HfUser.T (t.hfid, t.realm, author)
    )
  )
end
