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
open System.Text.RegularExpressions
open System.Xml.XPath
open Aether
open Aether.Operators
open HtmlAgilityPack
open HfVault
open HfVault.Optics

type T = T of Theme.T array

let themeXpath =
  XPathExpression.Compile
    "//*[@class='categ']/a[starts-with(@href,'/forum')]"

let extractId (node:HtmlNode) =
  let url = node.GetAttributeValue("href", null) in
  // We assume that each node has a href attribute.
  let regex = Regex("/(\d+)/$").Match(url) in
  if regex.Success
  then regex.Groups.[1].Value^.(Prism.ofEpimorphism String.int32_)
  else None

let extractName (node:HtmlNode) = Some (node^.HtmlNode.innerText_)

let extractTheme locale (node:HtmlNode) =
  (extractId node, extractName node)
  ||> Option.map2
      ( fun id name ->
          Theme.new_
          |> locale^=Theme.locale_
          |> id^=Theme.id_
          |> name^=Theme.name_
      )

let load (web:HtmlWeb) locale =
  let uri = UriBuilder(locale^.Locale.host_, Path="forum.html") in
  let themeNodes_ = HtmlWeb.get_ uri.Uri
                >?> HtmlDocument.root_
                >?> HtmlNode.nodes_ themeXpath in
  web^.themeNodes_
  |> Option.map
       ( Seq.map (extractTheme locale)
      >> Seq.collect Option.toList
      >> Seq.toArray
      >> T
       )
