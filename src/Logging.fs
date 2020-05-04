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

module HfVault.Logging
#nowarn "62"
#light "off"

open Aether
open Aether.Operators
open Hopac
open Logary
open Logary.Configuration
open Logary.Targets

let logary =
  Config.create "HfVault" "dotnet"
  |> Config.target (LiterateConsole.create LiterateConsole.empty "console")
  |> Config.ilogger (ILogger.Console Verbose)
  |> Config.build
  |> run

let getLogger (name:PointName) = logary.getLogger name

let stop () = logary.shutdown() |> run

let log (logger:Logger) level message =
  Message.eventX message level |> logger.logSimple

let logThemes (logger:Logger) themes = job
{ let m =
    Message.eventFormat
      ( Info
      , "Load {length} themes: {@names}"
      , Array.length themes
      , Array.map (Optic.get Forum.Theme.name_) themes
      )
    |> Message.tag "theme"
    |> Message.tag "load"
  in
  let! _ = logger.logWithAck(true, m.level) (fun _ -> m) in
  return ()
}

let logThreads (logger:Logger) threads = job
{ let m =
    Message.eventFormat
      ( Info
      , "Load {length} threads: {@names}"
      , Seq.length threads
      , Seq.map (Optic.get Forum.Thread.name_) threads
      )
    |> Message.tag "thread"
    |> Message.tag "load"
    |> Message.setName logger.name
  in
  let! _ = logger.logWithAck(true, m.level) (fun _ -> m) in
  return ()
}

let logPosts (logger:Logger) posts = job
{ let m =
    Message.eventFormat(Info, "Load {length} posts", Seq.length posts)
    |> Message.tag "post"
    |> Message.tag "load"
    |> Message.setName logger.name
  in
  let! _ = logger.logWithAck(true, m.level) (fun _ -> m) in
  return ()
}

let logInsertThemeSuccess (logger:Logger) hfTheme = job
{ let m =
    Message.eventFormat
      ( Info
      , "Insert theme {name} ({hfid}) with id {id}"
      , hfTheme^.(Domain.HfTheme.theme_ >-> Domain.Theme.name_)
      , hfTheme^.Domain.HfTheme.hfid_
      , hfTheme^.(Domain.HfTheme.theme_ >-> Domain.Theme.id_)
      )
    |> Message.tag "theme"
    |> Message.tag "insert"
    |> Message.tag "success"
    |> Message.setName logger.name
  in
  let! _ = logger.logWithAck(true, m.level) (fun _ -> m) in
  return ()
}

let logInsertUserSuccess (logger:Logger) hfUser = job
{ let m =
    Message.eventFormat
      ( Info
      , "Insert user {name} ({hfid}) with id {id}"
      , hfUser^.(Domain.HfUser.author_ >-> Domain.Author.name_)
      , hfUser^.Domain.HfUser.hfid_
      , hfUser^.(Domain.HfUser.author_ >-> Domain.Author.id_)
      )
    |> Message.tag "user"
    |> Message.tag "insert"
    |> Message.tag "success"
    |> Message.setName logger.name
  in
  let! _ = logger.logWithAck(true, m.level) (fun _ -> m) in
  return ()
}

let logInsertThreadSuccess (logger:Logger) hfThread = job
{ let m =
    Message.eventFormat
      ( Info
      , "Insert thread {name} ({hfid}) with id {id}"
      , hfThread^.(Domain.HfThread.thread_ >-> Domain.Thread.name_)
      , hfThread^.Domain.HfThread.hfid_
      , hfThread^.(Domain.HfThread.thread_ >-> Domain.Thread.id_)
      )
    |> Message.tag "thread"
    |> Message.tag "insert"
    |> Message.tag "success"
    |> Message.setName logger.name
  in
  let! _ = logger.logWithAck(true, m.level) (fun _ -> m) in
  return ()
}

let logInsertPostSuccess (logger:Logger) hfPost = job
{ let m =
    let postAuthorName_ = Domain.HfPost.post_
                      >-> Domain.Post.author_
                      >-> Domain.Author.name_ in
    Message.eventFormat
      ( Info
      , "Insert post from {authorName} with id {id}"
      , hfPost^.postAuthorName_
      , hfPost^.(Domain.HfPost.post_ >-> Domain.Post.id_)
      )
    |> Message.tag "post"
    |> Message.tag "insert"
    |> Message.tag "success"
    |> Message.setName logger.name
  in
  let! _ = logger.logWithAck(true, m.level) (fun _ -> m) in
  return ()
}
