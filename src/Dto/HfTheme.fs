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
module HfTheme = begin
  open Aether.Operators
  open HfVault

  type T = T of int * Realm.T * Theme.T

  type DbRepr = { hfid : int
                ; realm : DbTypes.Realm
                ; theme : Theme.DbRepr
                }

  let toDb (T (hfid, realm, theme)) =
     { hfid=hfid
     ; realm=realm^.Realm.dbRealm_
     ; theme=theme |> Theme.toDb
     }
end

namespace HfVault.Domain
module HfTheme = begin
  open HfVault

  type T = { hfid : int
           ; realm : Realm.T
           ; theme : Theme.T
           }

  let new_ = {hfid=0; realm=Unchecked.defaultof<_>; theme=Theme.new_}
  let hfid_ = (fun {hfid=h} -> h), (fun h t -> {t with hfid=h})
  let realm_ = (fun {realm=r} -> r), (fun r t -> {t with realm=r})
  let theme_ = (fun {theme=th} -> th), (fun th t -> {t with theme=th})

  let dto_ =
  ( ( fun (Dto.HfTheme.T (hfid, realm, theme)) ->
        let theme = theme |> (fst Theme.dto_) in
        {hfid=hfid; realm=realm; theme=theme}
    )
  , ( fun t ->
        let theme = t.theme |> (snd Theme.dto_) in
        Dto.HfTheme.T (t.hfid, t.realm, theme)
    )
  )
end
