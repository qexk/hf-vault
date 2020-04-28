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
module HfThread = begin
  open Aether.Operators
  open HfVault

  type T = T of int * Realm.T * Thread.T

  type DbRepr = { hfid : int
                ; realm : DbTypes.Realm
                ; thread : Thread.DbRepr
                }

  let toDb (T (hfid, realm, thread)) =
     { hfid=hfid
     ; realm=realm^.Realm.dbRealm_
     ; thread=thread |> Thread.toDb
     }
end

namespace HfVault.Domain
module HfThread = begin
  open HfVault

  type T = { hfid : int
           ; realm : Realm.T
           ; thread : Thread.T
           }

  let new_ = {hfid=0; realm=Unchecked.defaultof<_>; thread=Thread.new_}
  let hfid_ = (fun {hfid=h} -> h), (fun h t -> {t with hfid=h})
  let realm_ = (fun {realm=r} -> r), (fun r t -> {t with realm=r})
  let thread_ = (fun {thread=th} -> th), (fun th t -> {t with thread=th})

  let dto_ =
  ( ( fun (Dto.HfThread.T (hfid, realm, thread)) ->
        let thread = thread |> (fst Thread.dto_) in
        {hfid=hfid; realm=realm; thread=thread}
    )
  , ( fun t ->
        let thread = t.thread |> (snd Thread.dto_) in
        Dto.HfThread.T (t.hfid, t.realm, thread)
    )
  )
end
