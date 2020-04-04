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

module HfVault.Forum.Thread
#nowarn "62"
#light "off"

open HfVault

type T = { locale : Locale.T
         ; id : int
         ; name : string
         ; lastYear : int
         }

let new_ = {locale=Unchecked.defaultof<_>;id=0;name=null;lastYear=0}

let locale_ = (fun {locale=l} -> l), (fun l t -> {t with locale=l})
let id_ = (fun {id=i} -> i), (fun i t -> {t with id=i})
let name_ = (fun {name=n} -> n), (fun n t -> {t with name=n})
let lastYear_ = (fun {lastYear=l} -> l), (fun l t -> {t with lastYear=l})
