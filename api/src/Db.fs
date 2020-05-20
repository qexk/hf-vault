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

let ``select thread from realm`` realm hfid = job
{ use select = DbTypes.Db.CreateCommand<"""
    SELECT hf_thread.hfid, hf_user.author, author.name author_name,
           thread.created_at, thread.updated_at, hf_theme.hfid theme,
           thread.name, thread.open, thread.sticky,
           (SELECT COUNT(*)
              FROM post
             WHERE post.thread = thread.id) posts
      FROM thread
           JOIN hf_thread
           ON hf_thread.thread = thread.id
           JOIN hf_theme
           ON hf_theme.theme = thread.theme
           JOIN author
           ON author.id = thread.author
           JOIN hf_user
           ON hf_user.author = author.id
     WHERE hf_thread.realm = @realm
           AND hf_thread.hfid = @hfid
  """, SingleRow=true>(connRuntime) in
  let dtoRealm = realm |> (snd (Domain.Realm.dto_())) in
  let dbRealm = dtoRealm.value in
  match! select.AsyncExecute(realm=dbRealm, hfid=hfid) with
  | None     -> return None
  | Some row -> return { Dto.Thread.thread=row.hfid
                       ; Dto.Thread.realm=dtoRealm
                       ; Dto.Thread.theme=row.theme
                       ; Dto.Thread.posts=Option.defaultValue 0L row.posts
                       ; Dto.Thread.author=row.author
                       ; Dto.Thread.authorName=row.author_name
                       ; Dto.Thread.createdAt=row.created_at
                       ; Dto.Thread.updatedAt=row.created_at
                       ; Dto.Thread.name=row.name
                       ; Dto.Thread.``open``=row.``open``
                       ; Dto.Thread.sticky=row.sticky
                       } |> fst (Domain.Thread.dto_())
}

let ``select theme from realm`` realm hfid = job
{ use select = DbTypes.Db.CreateCommand<"""
      SELECT theme.name, COUNT(thread.id) threads
        FROM theme
             LEFT JOIN hf_theme AS hf
             ON hf.theme = theme.id
             JOIN thread
             ON thread.theme = theme.id
       WHERE hf.realm = @realm
             AND hf.hfid = @hfid
    GROUP BY theme.name
  """, SingleRow=true>(connRuntime) in
  let dtoRealm = realm |> (snd (Domain.Realm.dto_())) in
  let dbRealm = dtoRealm.value in
  match! select.AsyncExecute(realm=dbRealm, hfid=hfid) with
  | None     -> return None
  | Some row -> return { Dto.Theme.name=row.name
                       ; Dto.Theme.hfid=hfid
                       ; Dto.Theme.realm=dtoRealm
                       ; Dto.Theme.threads=Option.defaultValue 0L row.threads
                       } |> fst (Domain.Theme.dto_())
}

let ``select all themes from realm`` realm = job
{ use select = DbTypes.Db.CreateCommand<"""
      SELECT theme.name, hf.hfid, COUNT(thread.id) threads
        FROM theme
             JOIN hf_theme AS hf
             ON theme.id = hf.theme
             JOIN thread
             ON thread.theme = theme.id
       WHERE hf.realm = @realm
    GROUP BY theme.name, hf.hfid;
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
                     ; Dto.Theme.threads=Option.defaultValue 0L theme.threads
                     }
                 ) in
  let toDomain =
    Domain.List<Dto.Theme, Domain.Theme>.dto_<Dto.Theme, Domain.Theme>()
    |> fst
  in
  return {Dto.List.list=themes} |> toDomain
}

let ``select all posts from thread`` realm theme thread limit offset = job
{ use select = DbTypes.Db.CreateCommand<"""
      SELECT post.id, post.created_at, post.message,
             hf_user.hfid author, author.name author_name
        FROM thread
             JOIN hf_thread
             ON hf_thread.thread = thread.id
             JOIN hf_theme
             ON hf_theme.theme = thread.theme
             JOIN post
             ON post.thread = thread.id
             JOIN author
             ON author.id = post.author
             JOIN hf_user
             ON hf_user.author = author.id
       WHERE hf_thread.realm = @realm
             AND hf_theme.hfid = @theme
             AND hf_thread.hfid = @thread
    ORDER BY post.created_at ASC
       LIMIT @limit
      OFFSET @offset;
  """>(connRuntime) in
  let dtoRealm = realm |> snd (Domain.Realm.dto_()) in
  let dbRealm = dtoRealm.value in
  let limit = match limit with
              | Some l when l > 0 && l < 50 -> int64 l
              | _                           -> 50L in
  let offset = defaultArg offset 0 |> int64 in
  let! rows =
    select.AsyncExecute
      ( realm=dbRealm
      , theme=theme
      , thread=thread
      , limit=limit
      , offset=offset
      )
  in
  let posts = rows
           |> List.map
                ( fun post ->
                    { Dto.Post.post=post.id
                    ; Dto.Post.author=post.author
                    ; Dto.Post.authorName=post.author_name
                    ; Dto.Post.createdAt=post.created_at
                    ; Dto.Post.message=post.message
                    }
                )
  in
  let toDomain =
    Domain.List<Dto.Post, Domain.Post>.dto_<Dto.Post, Domain.Post>()
    |> fst
  in
  return {Dto.List.list=posts} |> toDomain
}

let ``select all threads from theme`` realm theme limit offset = job
{ use select = DbTypes.Db.CreateCommand<"""
      SELECT hf_thread.hfid, hf_user.author, author.name author_name,
             thread.created_at, thread.updated_at, thread.theme, thread.name,
             thread.open, thread.sticky
        FROM thread
             JOIN hf_thread
             ON hf_thread.thread = thread.id
             JOIN hf_theme
             ON hf_theme.theme = thread.theme
             JOIN author
             ON author.id = thread.author
             JOIN hf_user
             ON hf_user.author = author.id
       WHERE hf_thread.realm = @realm
             AND hf_theme.hfid = @theme
    ORDER BY thread.updated_at DESC
       LIMIT @limit
      OFFSET @offset
  """>(connRuntime) in
  let dtoRealm = realm |> snd (Domain.Realm.dto_()) in
  let dbRealm = dtoRealm.value in
  let limit = match limit with
              | Some l when l > 0 && l < 50 -> int64 l
              | _                           -> 50L in
  let offset = defaultArg offset 0 |> int64 in
  let! rows = select.AsyncExecute(realm=dbRealm, theme=theme, limit=limit, offset=offset) in
  let threads = rows
             |> List.map
                  ( fun thread ->
                      { Dto.Thread.thread=thread.hfid
                      ; Dto.Thread.realm=dtoRealm
                      ; Dto.Thread.theme=thread.theme
                      ; Dto.Thread.posts=(-1L)
                      ; Dto.Thread.author=thread.author
                      ; Dto.Thread.authorName=thread.author_name
                      ; Dto.Thread.createdAt=thread.created_at
                      ; Dto.Thread.updatedAt=thread.created_at
                      ; Dto.Thread.name=thread.name
                      ; Dto.Thread.``open``=thread.``open``
                      ; Dto.Thread.sticky=thread.sticky
                      }
                  ) in
  let toDomain =
    Domain.List<Dto.Thread, Domain.Thread>.dto_<Dto.Thread, Domain.Thread>()
    |> fst
  in
  return {Dto.List.list=threads} |> toDomain
}

let ``get stats of thread`` realm theme thread = job
{ use query = DbTypes.Db.CreateCommand<"""
      with all_posts AS (SELECT post.created_at, post.author
                           FROM hf_theme
                                JOIN theme
                                ON theme.id = hf_theme.theme
                                JOIN thread
                                ON thread.theme = theme.id
                                JOIN hf_thread
                                ON hf_thread.thread = thread.id
                                JOIN post
                                ON post.thread = thread.id
                          WHERE hf_theme.realm = @realm
                                AND hf_theme.hfid = @theme
                                AND hf_thread.hfid = @thread
                       ORDER BY post.created_at DESC)
    SELECT created_at last_update,
           hf_user.hfid author, author.name authorName,
           c.count posts
      FROM all_posts
           CROSS JOIN (SELECT COUNT(*) count FROM all_posts) AS c
           JOIN author
           ON author.id = all_posts.author
           JOIN hf_user
           ON hf_user.author = author.id
     LIMIT 1;
  """, SingleRow=true>(connRuntime) in
  let dtoRealm = realm |> (snd (Domain.Realm.dto_())) in
  let dbRealm = dtoRealm.value in
  match! query.AsyncExecute(theme=theme, realm=dbRealm, thread=thread) with
  | None -> return None
  | Some row -> return row.posts
                    |> Option.map
                         ( fun posts ->
                             { Dto.ThreadStats.posts=posts
                             ; Dto.ThreadStats.lastUpdate=row.last_update
                             ; Dto.ThreadStats.author=row.author
                             ; Dto.ThreadStats.authorName=row.authorname
                             } |> fst Domain.ThreadStats.dto_
                         )
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
