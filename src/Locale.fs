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

module HfVault.Locale
#nowarn "62"
#light "off"

open Aether

type T = FR

let host_ : Lens<T, System.Uri> =
  ( ( function
      | FR -> System.Uri("http://www.hammerfest.fr")
    )
  , fun _ _ -> failwith "unimplemented"
  )

let globalization_ : Lens<T, System.Globalization.CultureInfo> =
  ( ( function
      | FR -> System.Globalization.CultureInfo("FR")
    )
  , fun _ _ -> failwith "unimplemented"
  )
