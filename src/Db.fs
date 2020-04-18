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

module HfVault.Db
#nowarn "62"
#light "off"

open Aether.Operators

let connRuntime =
  try
    let stream = System.IO.File.OpenRead("config.json") in
    let root = System.Text.Json.JsonDocument.Parse(stream) in
    root.RootElement.GetProperty("ConnectionStrings")
                    .GetProperty("hf-vault")
                    .GetString()
  with
  | _ -> failwith "Configuration file ‘config.json’ not found or invalid"

let insertHfThemeAndTheme hfTheme = async
{ use cmd = DbTypes.Db.CreateCommand<"""
           WITH new_theme AS (INSERT INTO theme (name)
                                   VALUES (@name)
                                RETURNING id)
    INSERT INTO hf_theme (hfid, realm, theme)
         VALUES (@hfid, @realm, (SELECT id FROM new_theme))
      RETURNING theme
  """, SingleRow=true>(connRuntime) in
  let dto = hfTheme |> (snd Domain.HfTheme.dto_) |> Dto.HfTheme.toDb in
  let themeId_ = Domain.HfTheme.theme_ >-> Domain.Theme.id_ in
  try
    let! id =
      cmd.AsyncExecute(name=dto.theme.name, hfid=dto.hfid, realm=dto.realm)
    in
    return id |> Option.map (fun id -> hfTheme |> id^=themeId_)
  with
  | _ -> return None
}
