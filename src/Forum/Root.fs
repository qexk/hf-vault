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

module HfVault.Forum.Root
#nowarn "62"
#light "off"

open System
open System.Xml.XPath
open Aether
open Aether.Operators
open HtmlAgilityPack
open HfVault
open HfVault.Optics

type T = T of Theme.T array

let themes_ : Lens<_, _> =
  (fun (T themes) -> themes), (fun themes _ -> T themes)

let themeXpath =
  XPathExpression.Compile
    "//*[@class='categ']/a[starts-with(@href,'/forum')]"

let extractId = Util.extractLinkId

let extractName = Optic.get HtmlNode.innerText_

let extractDesc (node:HtmlNode) =
  try
    match node.ParentNode
              .ParentNode
              .SelectSingleNode("*[@class='categDesc']") with
    | null -> ""
    | node -> node.InnerText
  with
  | _ -> ""

let extractTheme realm (node:HtmlNode) =
  extractId node
  |> Option.map
       ( fun id ->
           Theme.new_
           |> realm^=Theme.realm_
           |> id^=Theme.id_
           |> (extractName node)^=Theme.name_
           |> (extractDesc node)^=Theme.desc_
       )

let load (web:HtmlWeb) realm =
  let uri = UriBuilder(realm^.Realm.host_, Path="forum.html") in
  let themeNodes_ = HtmlWeb.get_ uri.Uri
                >?> HtmlDocument.root_
                >?> HtmlNode.nodes_ themeXpath in
  web^.themeNodes_
  |> Option.map
       ( Seq.map (extractTheme realm)
      >> Seq.collect Option.toList
      >> Seq.toArray
      >> T
       )
