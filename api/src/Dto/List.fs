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

namespace Api
#nowarn "62"
#light "off"

namespace Api.Dto
open Thoth.Json.Net

type List< ^T when ^T : (static member jsonEncoder : ^T -> JsonValue)> =
  { list : ^T list
  } with
  static member inline jsonEncoder<
    ^T when ^T : (static member jsonEncoder : ^T -> JsonValue)
  >(xx) =
    let mutable list = [] in
    for t in xx.list do
      list <- (^T : (static member jsonEncoder : ^T -> JsonValue)(t))::list
    done;
    Encode.list list

end

namespace Api.Domain
open Thoth.Json.Net
open Api

type List<
  ^Dto, ^T
  when ^Dto : (static member jsonEncoder : ^Dto -> JsonValue)
  and ^T : (static member dto_ : unit -> (^Dto -> ^T option) * (^T -> ^Dto))
> = {list : ^T list} with
  static member inline get<
    ^Dto, ^T
    when ^Dto : (static member jsonEncoder : ^Dto -> JsonValue)
    and ^T : (static member dto_ : unit -> (^Dto -> ^T option) * (^T -> ^Dto))
  >(dto:Dto.List< ^Dto>) =
    let mutable list = [] in
    let inline toDomain dto =
      (^T : (static member dto_ : unit -> (^Dto -> ^T option) * (^T -> ^Dto))())
      |> fst
      <| dto
    in
    for dto in dto.list do
      list <- toDomain dto::list
    done;
    {list=list |> List.choose id}

  static member inline set<
    ^Dto, ^T
    when ^Dto : (static member jsonEncoder : ^Dto -> JsonValue)
    and ^T : (static member dto_ : unit -> (^Dto -> ^T option) * (^T -> ^Dto))
  >(t:List< ^Dto, ^T>) =
    let mutable list = [] in
    let inline toDto t =
      (^T : (static member dto_ : unit -> (^Dto -> ^T option) * (^T -> ^Dto))())
      |> snd
      <| t
    in
    for t in t.list do
      list <- toDto t::list
    done;
    {Dto.List.list=list}

  static member inline dto_<
    ^Dto, ^T
    when ^Dto : (static member jsonEncoder : ^Dto -> JsonValue)
    and ^T : (static member dto_ : unit -> (^Dto -> ^T option) * (^T -> ^Dto))
  >() =
    ( List< ^Dto, ^T>.get< ^Dto, ^T>
    , List< ^Dto, ^T>.set< ^Dto, ^T>
    )
end
