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

module HfVault.Forum.Thread
#nowarn "62"
#light "off"

open System
open System.Xml.XPath
open Aether.Operators
open HfVault
open HfVault.Optics

type T = { realm : Realm.T
         ; id : int
         ; name : string
         ; lastYear : int
         }

let new_ = {realm=Unchecked.defaultof<_>;id=0;name=null;lastYear=0}

let realm_ = (fun {realm=l} -> l), (fun l t -> {t with realm=l})
let id_ = (fun {id=i} -> i), (fun i t -> {t with id=i})
let name_ = (fun {name=n} -> n), (fun n t -> {t with name=n})
let lastYear_ = (fun {lastYear=l} -> l), (fun l t -> {t with lastYear=l})

let messageXpath = XPathExpression.Compile "//div[@class='message']"

let paginationXpath =
  XPathExpression.Compile
    "//div[@class='forum']/div[@class='paginateBox']/div[@class='paginate']/a[@href]"

let rec loadRawPosts web t acc page =
  let url = UriBuilder(t.realm^.Realm.host_) in
  url.Path <- sprintf "/forum.html/thread/%i" t.id;
  url.Query <- sprintf "?page=%i" page;
  match web^.HtmlWeb.get_ url.Uri with
  | None     -> acc
  | Some doc -> let root = doc^.HtmlDocument.root_ in
                let posts = root^.(HtmlNode.nodes_ messageXpath) in
                let acc = Seq.append acc posts in
                let nextPage = sprintf "page=%i" (page + 1) in
                if root^.(HtmlNode.nodes_ paginationXpath)
                   |> Seq.exists
                      ( fun link ->
                          link.GetAttributeValue("href", "").EndsWith(nextPage)
                      )
                then loadRawPosts web t acc (page + 1)
                else acc

let messageDateXpath =
  XPathExpression.Compile
    "div[@class='header']/div[@class='date']"

let messageAuthorXpath =
  XPathExpression.Compile
    "div[@class='header']/div[@class='author']/a[@href]"

let messageContentXpath = XPathExpression.Compile "div[@class='content']"

let applyDate t posts =
  let g = t^.(realm_ >-> Realm.globalization_) in
  let mutable acc = [] in
  let mutable year = t.lastYear in
  let mutable date = Unchecked.defaultof<_> in
  for i = (Array.length posts) - 1 downto 0 do
    let node = posts.[i] in
    let dateChunkNode = node^.(HtmlNode.node_ messageDateXpath) in
    let authorId =
      node^.(HtmlNode.node_ messageAuthorXpath)
      |> Option.bind Util.extractLinkId
    in
    let contentNode = node^.(HtmlNode.node_ messageContentXpath) in
    if dateChunkNode.IsSome && authorId.IsSome && contentNode.IsSome
    then let dateChunk = dateChunkNode.Value.InnerText in
         let authorId = authorId.Value in
         let content = contentNode.Value.InnerHtml in
         let () =
           while
             let repr = sprintf "%s%i" dateChunk year in
               if DateTime.TryParseExact
                  ( repr
                  , "dddd dd MMMM HH:mmyyyy"
                  , g
                  , Globalization.DateTimeStyles.None
                  , &date
                  )
               then false
               else DateTime.TryParseExact
                    ( repr
                    , "dddd dd MMM HH:mmyyyy"
                    , g
                    , Globalization.DateTimeStyles.None
                    , &date
                    ) |> not
           do year <- year - 1 done
         in
         acc <- {|date=date;content=content;author=authorId|}::acc
  done;
  acc

let makePost realm (data:{| author: int; content: string; date: DateTime |}) =
  Post.new_
  |> realm^=Post.realm_
  |> data.author^=Post.author_
  |> data.date^=Post.createdAt_
  |> data.content^=Post.content_

let load web t =
  let rawPosts = loadRawPosts web t [] 1 |> Seq.toArray in
  let postData = applyDate t rawPosts in
  List.map (makePost t.realm) postData
