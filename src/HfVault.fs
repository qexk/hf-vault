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

module HfVault.__main__
#nowarn "62"
#light "off"

open Aether
open Aether.Operators
open Hopac
open Logary
open HtmlAgilityPack
open HfVault
open HfVault.Optics

let userAgent = "hf-vault-bot/1.0.0 (+https://github.com/Aksamyt/hf-vault)"

type Options = { realm : Realm.T
               ; mustExit : bool
               } with
  static member new_ = { realm=Realm.FR
                       ; mustExit=false
                       }

  static member usage = """Usage:
  dotnet <path-to-hf-vault> [options]

Options:
  -r, --realm REALM  Set the realm to scrape.
                     [type: FR | EN | ES, default: FR]
"""
end

let rec parseArgv o = function
| ("-r"|"--realm")::r::tl ->
    begin match r^.(Prism.ofEpimorphism String.realm_) with
    | Some r -> parseArgv {o with realm=r} tl
    | None   -> Result.Error (r + ": invalid realm")
    end
| ("-h"|"--help")::_ -> Ok {o with mustExit=true}
| jaj::_ -> Result.Error (jaj + ": unknown option")
| []     -> Ok o

/// THICC boi
let insertThread theme forumThread forumPosts = job
{ let logger =
    PointName.ofArray
      [|theme^.Domain.Theme.name_; forumThread^.Forum.Thread.name_|]
    |> Logging.getLogger
  in
  do! Logging.logPosts logger forumPosts in
  match forumPosts with
  | [||]  -> return ()
  | posts ->
      match!
        posts.[0]
        |> Forum.Post.makeNewHfUser
        |> Db.``insert HfUser and Author or get id``
      with
      | None             -> Logging.log logger Warn "Error inserting author"
      | Some firstAuthor ->
          do! Logging.logInsertUserSuccess logger firstAuthor in
          let firstPostDate = posts.[0]^.Forum.Post.createdAt_ in
          let lastPostDate = posts.[posts.Length - 1]^.Forum.Post.createdAt_ in
          let thread =
            Domain.Thread.new_
            |> (firstAuthor^.Domain.HfUser.author_)^=Domain.Thread.author_
            |> firstPostDate^=Domain.Thread.createdAt_
            |> lastPostDate^=Domain.Thread.updatedAt_
            |> theme^=Domain.Thread.theme_
            |> (forumThread^.Forum.Thread.name_)^=Domain.Thread.name_
            |> true^=Domain.Thread.open_
            |> false^=Domain.Thread.sticky_
          in
          let hfThread =
            Domain.HfThread.new_
            |> (forumThread^.Forum.Thread.realm_)^=Domain.HfThread.realm_
            |> (forumThread^.Forum.Thread.id_)^=Domain.HfThread.hfid_
            |> thread^=Domain.HfThread.thread_
          in
          match! Db.``insert HfThread and Thread or get id`` hfThread with
          | None          -> Logging.log logger Warn "Error inserting thread"
          | Some hfThread ->
              do! Logging.logInsertThreadSuccess logger hfThread in
              let threadId =
                hfThread^.(Domain.HfThread.thread_ >-> Domain.Thread.id_)
              in
              for ix = 0 to forumPosts.Length - 1 do
                let post = forumPosts.[ix] in
                let logger =
                  PointName.ofArray
                    [| theme^.Domain.Theme.name_
                     ; forumThread^.Forum.Thread.name_
                     ; sprintf "%i/%i" (ix + 1) forumPosts.Length
                    |]
                  |> Logging.getLogger
                in
                match!
                  post
                  |> Forum.Post.makeNewHfUser
                  |> Db.``insert HfUser and Author or get id``
                with
                | None        -> Logging.log logger Warn "Error inserting user"
                | Some hfUser ->
                    let domainPost =
                      Domain.Post.new_
                      |> (hfUser^.Domain.HfUser.author_)^=Domain.Post.author_
                      |> (post^.Forum.Post.createdAt_)^=Domain.Post.createdAt_
                      |> (post^.Forum.Post.content_)^=Domain.Post.message_
                      |> threadId^=Domain.Post.thread_
                    in
                    let hfPost =
                      Domain.HfPost.new_
                      |> (post^.Forum.Post.id_)^=Domain.HfPost.hfid_
                      |> (post^.Forum.Post.realm_)^=Domain.HfPost.realm_
                      |> domainPost^=Domain.HfPost.post_
                    in
                    try
                      match! Db.``insert new HfPost and Post`` hfPost with
                      | None -> Logging.log logger Warn "Error inserting post"
                      | Some hfPost ->
                          do! Logging.logInsertPostSuccess logger hfPost in
                          ()
                    with
                    | _ -> Logging.log logger Warn "Error inserting post"
              done;
              return ()
}

let insertTheme forumTheme =
  let theme =
    Domain.Theme.new_
    |> (forumTheme^.Forum.Theme.name_)^=Domain.Theme.name_
    |> (forumTheme^.Forum.Theme.desc_)^=Domain.Theme.desc_
  in
  let hfTheme =
    Domain.HfTheme.new_
    |> (forumTheme^.Forum.Theme.id_)^=Domain.HfTheme.hfid_
    |> (forumTheme^.Forum.Theme.realm_)^=Domain.HfTheme.realm_
    |> theme^=Domain.HfTheme.theme_
  in
  Db.``insert HfTheme and Theme or get id`` hfTheme

let ``scrape threads`` web forumTheme = job
{ let logger =
    Logging.getLogger (PointName.ofSingle (forumTheme^.Forum.Theme.name_))
  in
  match! insertTheme forumTheme with
  | None         -> Logging.log logger Warn "Error inserting"
  | Some hfTheme ->
      do! Logging.logInsertThemeSuccess logger hfTheme in
      let theme = hfTheme^.Domain.HfTheme.theme_ in
      let forumThreads = Forum.Theme.load web forumTheme in
      do! Logging.logThreads logger forumThreads in
      for forumThread in forumThreads do
        let forumPosts = Forum.Thread.load web forumThread |> List.toArray in
        do! insertThread theme forumThread forumPosts in
        ()
      done
}

let ``scrape it!`` o = job
{ let logger = Logging.getLogger (PointName.ofSingle "root") in
  let web = HtmlWeb(UserAgent=userAgent) in
  let themes =
    match Forum.Root.load web o.realm with
    | Some root -> root^.Forum.Root.themes_
    | _         -> failwith "error loading forum root"
  in
  do! Logging.logThemes logger themes in
  Logging.log logger Info "Begin parallelized work";
  do! themes |> Array.map (``scrape threads`` web) |> Job.conIgnore in
  Logging.log logger Info "End parallelized work"
}

[<EntryPoint>]
let main argv =
  use _quitLogary =
    {new System.IDisposable with member _.Dispose() = Logging.stop ()}
  in
  let logger = Logging.getLogger (PointName.ofSingle "main") in
  try
    match parseArgv Options.new_ (Array.toList argv) with
    | Result.Error e     -> failwith e
    | Ok {mustExit=true} -> System.Console.Write(Options.usage); 0
    | Ok o               -> ``scrape it!`` o |> run; 0
  with
  | e -> Message.eventError "top-level exception"
         |> Message.addExn e
         |> logger.logSimple;
         -1
