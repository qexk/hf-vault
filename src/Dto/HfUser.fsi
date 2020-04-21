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

namespace HfVault.Dto
  module HfUser =
    type T

    type DbRepr = { hfid : int
                  ; realm : HfVault.DbTypes.Realm
                  ; author : Author.DbRepr
                  }

    val toDb : T
            -> DbRepr

namespace HfVault.Domain
  module HfUser =
    type T

    val new_ : T
    val hfid_ : Aether.Lens<T, int>
    val realm_ : Aether.Lens<T, HfVault.Realm.T>
    val author_ : Aether.Lens<T, Author.T>

    val dto_ : Aether.Isomorphism<HfVault.Dto.HfUser.T, T>
