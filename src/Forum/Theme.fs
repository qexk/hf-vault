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

module HfVault.Forum.Theme
#nowarn "62"
#light "off"

open System
open System.Xml.XPath
open Aether.Operators
open HfVault
open HfVault.Optics

type T = { locale : Locale.T
         ; id : int
         ; name : string
         }

let new_ = {locale=Unchecked.defaultof<_>;id=0;name=null}

let locale_ = (fun {locale=l} -> l), (fun l t -> {t with locale=l})

let id_ = (fun {id=i} -> i), (fun i t -> {t with id=i})

let name_ = (fun {name=n} -> n), (fun n t -> {t with name=n})

let rowsXpath =
  XPathExpression.Compile
    "//table[@class='threads']/tr[not(contains(@class,'sticky')) and not(th)]"

let rec loadPage web t acc page =
  let url = UriBuilder(t.locale^.Locale.host_) in
  url.Path <- sprintf "/forum.html/theme/%i" t.id;
  url.Query <- sprintf "?page=%i" page;
  match web^.HtmlWeb.get_ url.Uri with
  | None     -> acc
  | Some doc -> let root = doc^.HtmlDocument.root_ in
                let rows = root^.(HtmlNode.nodes_ rowsXpath) in
                if isNull rows || Seq.isEmpty rows
                then acc
                else loadPage web t (Seq.append acc rows) (page + 1)

let forumDateXpath = XPathExpression.Compile "./td[@class='forumDate']"

let (|Date|_|) node =
  node^.(HtmlNode.node_ forumDateXpath)
  |> Option.map (fun date -> date.InnerText.Trim())

let forumThreadXpath =
  XPathExpression.Compile
    "self::node()[contains(@class,'thread')]/td[@class='subject']/a[@href]"

let (|Thread|_|) node =
  match node^.(HtmlNode.node_ forumThreadXpath) with
  | None      -> None
  | Some link -> Util.extractLinkId link
                 |> Option.map (fun id -> {|id=id;name=link.InnerText|})

let applyYear t rows =
  let g = t^.(locale_ >-> Locale.globalization_) in
  let mutable year = DateTime.UtcNow.Year in
  let mutable acc = [] in
  use iter = rows^.Seq.enumerator_ in
  while iter.MoveNext() do
    match iter.Current with
    | Thread thread -> acc <- {|thread with year=year|}::acc
    | Date text     -> while
                         let date = sprintf "%s%i" text year in
                         try
                          DateTime.ParseExact(date, "dddd d MMMMyyyy", g)
                          |> fun _ -> false
                         with
                         | _ -> true
                       do year <- year - 1 done
    | _             -> () //TODO: log info
  done;
  acc

let makeThread locale (data:{| id: int32; name: string; year: int |}) =
  Thread.new_
  |> locale^=Thread.locale_
  |> data.id^=Thread.id_
  |> data.name^=Thread.name_
  |> data.year^=Thread.lastYear_

let load web t =
  let rows = loadPage web t [] 1 in
  let threadData = applyYear t rows in
  List.map (makeThread t.locale) threadData
