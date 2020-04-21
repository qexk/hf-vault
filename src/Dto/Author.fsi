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
  module Author =
    type T

    type DbRepr = { id : int
                  ; name : string
                  }

    val toDb : T
            -> DbRepr

    val fromDb : DbRepr
              -> T

namespace HfVault.Domain
  module Author =
    type T

    val new_ : T
    val id_ : Aether.Lens<T, int>
    val name_ : Aether.Lens<T, string>

    val dto_ : Aether.Isomorphism<HfVault.Dto.Author.T, T>
