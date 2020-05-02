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
open HtmlAgilityPack
open HfVault
open HfVault.Optics

let userAgent = "hf-vault-bot/0.0.0 (+https://github.com/Aksamyt/hf-vault)"

type Options = { realm : Realm.T
               ; mustExit : bool
               } with
  static member new_ = { realm=Realm.FR
                       ; mustExit=false
                       }

  static member usage = """Usage:
  dotnet <path-to-hf-vault> [options]

Options:
  -r REALM, --realm REALM  Set the realm to scrape.
                           [type: FR | EN | ES, default: FR]
"""
end

let rec parseArgv o = function
| ("-r"|"--realm")::r::tl ->
    begin match r^.(Prism.ofEpimorphism String.realm_) with
    | Some r -> parseArgv {o with realm=r} tl
    | None   -> Error (r + ": invalid realm")
    end
| ("-h"|"--help")::_ -> Ok {o with mustExit=true}
| jaj::_ -> Error (jaj + ": unknown option")
| []     -> Ok o

let insertThread theme forumThread forumPosts = async
{ match forumPosts with
  | [||]  -> return ()
  | posts ->
      match!
        posts.[0]
        |> Forum.Post.makeNewHfUser
        |> Db.``insert HfUser and Author or get id``
      with
      | None             -> return ()
      | Some firstAuthor ->
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
          match! Db.``insert new HfThread and Thread`` hfThread with
          | None             -> return ()
          | Some hfThread ->
              let threadId =
                hfThread^.(Domain.HfThread.thread_ >-> Domain.Thread.id_)
              in
              for post in forumPosts do
                match!
                  post
                  |> Forum.Post.makeNewHfUser
                  |> Db.``insert HfUser and Author or get id``
                with
                | None        -> ()
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
                    let! _ = Db.``insert new HfPost and Post`` hfPost in
                    ()
              done;
              return ()
}

let insertTheme forumTheme =
  let theme =
    Domain.Theme.new_
    |> (forumTheme^.Forum.Theme.name_)^=Domain.Theme.name_
  in
  let hfTheme =
    Domain.HfTheme.new_
    |> (forumTheme^.Forum.Theme.id_)^=Domain.HfTheme.hfid_
    |> (forumTheme^.Forum.Theme.realm_)^=Domain.HfTheme.realm_
    |> theme^=Domain.HfTheme.theme_
  in
  Db.insertHfThemeAndTheme hfTheme

let ``scrape it!`` o = async
{ let web = HtmlWeb(UserAgent="hf-vault") in
  let themes =
    match Forum.Root.load web o.realm with
    | Some root -> root^.Forum.Root.themes_
    | _         -> failwith "error loading forum root"
  in
  let work forumTheme = async
  { match! insertTheme forumTheme with
    | None         -> ()
    | Some hfTheme ->
        let theme = hfTheme^.Domain.HfTheme.theme_ in
        let forumThreads = Forum.Theme.load web forumTheme in
        for forumThread in forumThreads do
          let forumPosts = Forum.Thread.load web forumThread |> List.toArray in
          let! _ = insertThread theme forumThread forumPosts in
          ()
        done
  } in
  let! _ = themes |> Array.map work |> Async.Parallel in
  return ()
}

[<EntryPoint>]
let main argv =
  match parseArgv Options.new_ (Array.toList argv) with
  | Error e            -> Printf.eprintfn "error: %s" e; -1
  | Ok {mustExit=true} -> System.Console.Write(Options.usage); 0
  | Ok o               -> try ``scrape it!`` o |> Async.RunSynchronously; 0 with
                          | e -> Printf.eprintfn
                                   "fatal error: %s (%A)"
                                   e.Message
                                   e.TargetSite;
                                 -1
