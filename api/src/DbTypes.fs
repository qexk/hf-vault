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

module Api.DbTypes
#nowarn "62"
#light "off"

open FSharp.Data.Npgsql

[<Literal>] let private CONN_KEY = "hf-vault";;
[<Literal>] let private CONFIG = __SOURCE_DIRECTORY__ + "/development.settings.json";;

type Db = NpgsqlConnection<Connection=CONN_KEY, Config=CONFIG>

type Realm = Db.``public``.Types.realm
