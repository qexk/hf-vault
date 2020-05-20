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

namespace Api.Machine.Forum.Themes.Threads
#nowarn "62"
#light "off"

open Freya.Core
open Freya.Core.Operators
open Freya.Machines.Http
open Freya.Machines.Http.Cors
open Freya.Routers.Uri.Template
open Freya.Types.Http
open Thoth.Json.Net
open Api

[<AutoOpen>]
module private __ThemeThreadPostsImpl__ = begin
  let parseInt (s:string) =
    let mutable res = 0 in
    if System.Int32.TryParse(s, &res)
    then Some res
    else None

  let q_ = Route.keys_ "q"

  let limit_ = freya
  { let! q = !.q_ in
    return q
    |> Option.bind (List.tryFind (fst >> ( = ) "limit"))
    |> Option.bind (snd >> parseInt)
  }

  let offset_ = freya
  { let! q = !.q_ in
    return q
    |> Option.bind (List.tryFind (fst >> ( = ) "offset"))
    |> Option.bind (snd >> parseInt)
  }

  let ``200`` = freya
  { let! realm = Option.get <!> Machine.Pervasives.realm in
    let! theme = Option.get <!> Machine.Pervasives.themeHfid in
    let! thread = Option.get <!> Machine.Pervasives.threadHfid in
    let! limit = limit_ in
    let! offset = offset_ in
    let! threads =
      Db.``select all posts from thread`` realm theme thread limit offset
      |> Freya.fromJob
    in
    let json = threads
            |> snd (Domain.List.dto_<Dto.Post, Domain.Post>())
            |> Dto.List.jsonEncoder<Dto.Post>
            |> Encode.toString 0 in
    return { Data=System.Text.Encoding.UTF8.GetBytes json
           ; Description={ Charset=Some Charset.Utf8
                         ; Encodings=None
                         ; MediaType=Some MediaType.Json
                         ; Languages=None
                         }
           }
  }

  let machine = freyaMachine
  { exists (Option.isSome <!> Machine.Pervasives.realm)
  ; exists (Option.isSome <!> Machine.Pervasives.themeHfid)
  ; exists (Option.isSome <!> Machine.Pervasives.threadHfid)
  ; handleOk ``200``
  ; cors
  }
end

type PostsMachine = Posts with
  static member Pipeline(_) = HttpMachine.Pipeline(machine)
end
