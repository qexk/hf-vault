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

module Api.Db
#nowarn "62"
#light "off"

open Hopac

let connRuntime =
  try
    let stream = System.IO.File.OpenRead("config.json") in
    let root = System.Text.Json.JsonDocument.Parse(stream) in
    root.RootElement.GetProperty("ConnectionStrings")
                    .GetProperty("hf-vault")
                    .GetString()
  with
  | _ -> failwith "Configuration file ‘config.json’ not found or invalid"

let ``select all stored realms`` () = job
{ use select = DbTypes.Db.CreateCommand<"""
    SELECT DISTINCT realm
               FROM hf_theme
  """>(connRuntime) in
  let! rows = select.AsyncExecute() in
  let realms = rows |> List.map (fun realm -> {Dto.Realm.value=realm}) in
  let toDomain =
    Domain.List<Dto.Realm, Domain.Realm>.dto_<Dto.Realm, Domain.Realm>()
    |> fst
  in
  return {Dto.List.list=realms} |> toDomain
}

let ``select all themes from realm`` realm = job
{ use select = DbTypes.Db.CreateCommand<"""
    SELECT theme.name, hf.hfid
      FROM theme
           LEFT JOIN hf_theme AS hf
           ON theme.id = hf.theme
     WHERE hf.realm = @realm
  """>(connRuntime) in
  let dtoRealm = realm |> (snd (Domain.Realm.dto_())) in
  let dbRealm = dtoRealm.value in
  let! rows = select.AsyncExecute(dbRealm) in
  let themes = rows
            |> List.map
                 ( fun theme ->
                     { Dto.Theme.name=theme.name
                     ; Dto.Theme.hfid=theme.hfid
                     ; Dto.Theme.realm=dtoRealm
                     }
                 ) in
  let toDomain =
    Domain.List<Dto.Theme, Domain.Theme>.dto_<Dto.Theme, Domain.Theme>()
    |> fst
  in
  return {Dto.List.list=themes} |> toDomain
}
}

let ``get stats of theme`` realm theme = job
{ use query = DbTypes.Db.CreateCommand<"""
     WITH all_posts AS (SELECT post.id post, post.created_at,
                               thread.id thread, thread.name thread_name,
                               post.author
                          FROM hf_theme
                               JOIN theme
                               ON theme.id = hf_theme.theme
                               JOIN thread
                               ON thread.theme = theme.id
                               JOIN post
                               ON post.thread = thread.id
                         WHERE hf_theme.hfid = @theme
                               AND hf_theme.realm = @realm),
          newest as (SELECT *
                       FROM all_posts
                   ORDER BY created_at DESC
                      LIMIT 1),
          count as (SELECT count(all_posts.post) posts,
                           count(distinct all_posts.thread) threads
                      FROM all_posts)
    SELECT count.posts, count.threads, newest.created_at last_update,
           hf_thread.hfid thread, newest.thread_name,
           hf_user.hfid author, author.name author_name
      FROM count, newest
           JOIN author
           ON author.id = newest.author
           JOIN hf_user
           ON hf_user.author = author.id
           JOIN hf_thread
           ON hf_thread.thread = newest.thread
  """, SingleRow=true>(connRuntime) in
  let dtoRealm = realm |> (snd (Domain.Realm.dto_())) in
  let dbRealm = dtoRealm.value in
  match! query.AsyncExecute(theme=theme, realm=dbRealm) with
  | None     -> return None
  | Some row -> return (row.posts, row.threads)
                ||> Option.map2
                      ( fun posts threads ->
                          { Dto.ThemeStats.posts=posts
                          ; Dto.ThemeStats.threads=threads
                          ; Dto.ThemeStats.lastUpdate=row.last_update
                          ; Dto.ThemeStats.thread=row.thread
                          ; Dto.ThemeStats.threadName=row.thread_name
                          ; Dto.ThemeStats.author=row.author
                          ; Dto.ThemeStats.authorName=row.author_name
                          }
                      )
                 |> Option.map (fst Domain.ThemeStats.dto_)
}
