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
module private __ThemeStatsImpl__ = begin
  let getStats = freya
  { let! realm = Option.get <!> Machine.Pervasives.realm in
    let! theme = Option.get <!> Machine.Pervasives.themeHfid in
    let! thread = Option.get <!> Machine.Pervasives.threadHfid in
    return! Db.``get stats of thread`` realm theme thread |> Freya.fromJob
  } |> Freya.memo

  let ``200`` = freya
  { let! stats = Option.get <!> getStats in
    let json = stats
            |> (snd Domain.ThreadStats.dto_)
            |> Dto.ThreadStats.jsonEncoder
            |> Encode.toString 0 in
    return { Data=System.Text.Encoding.UTF8.GetBytes json
           ; Description={ Charset=Some Charset.Utf8
                         ; Encodings=None
                         ; MediaType=Some MediaType.Json
                         ; Languages=None
                         }
           }
  }

  let ``400`` = freya {
    let! a = Machine.Pervasives.realm in
    let! b = Machine.Pervasives.themeHfid in
    let! c = Machine.Pervasives.threadHfid in
    let! d = getStats in
    return Represent.text (sprintf "%A\n%A\n%A\n%A" a b c d)
  }

  let machine = freyaMachine
  { exists (Option.isSome <!> Machine.Pervasives.realm)
  ; exists (Option.isSome <!> Machine.Pervasives.themeHfid)
  ; exists (Option.isSome <!> Machine.Pervasives.threadHfid)
  ; exists (Option.isSome <!> getStats)
  ; handleOk ``200``
  ; handleNotFound ``400``
  ; cors
  }
end

type StatsMachine = Stats with
  static member Pipeline(_) = HttpMachine.Pipeline(machine)
end
