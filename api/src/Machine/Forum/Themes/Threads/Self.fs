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
open Freya.Types.Http
open Thoth.Json.Net
open Api

[<AutoOpen>]
module private __ThreadSelfImpl__ = begin
  let thread = freya
  { let! realm = Machine.Pervasives.realm in
    let! hfid = Machine.Pervasives.threadHfid in
    match realm, hfid with
    | Some realm, Some hfid -> return! Db.``select thread from realm``
                                         realm
                                         hfid
                                    |> Freya.fromJob
    | _                     -> return None
  } |> Freya.memo

  let ``200`` = freya
  { let! thread = Option.get <!> thread in
    let json = thread
            |> snd (Domain.Thread.dto_())
            |> Dto.Thread.jsonEncoder
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
  { exists (Option.isSome <!> thread)
  ; handleOk ``200``
  ; cors
  }
end

type ThreadSelfMachine = Self with
  static member Pipeline(_) = HttpMachine.Pipeline(machine)
end
