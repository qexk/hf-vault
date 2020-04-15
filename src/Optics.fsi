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

namespace HfVault.Optics

open Aether
open HtmlAgilityPack

module HtmlWeb =
  val get_ : System.Uri -> Prism<HtmlWeb, HtmlDocument>

module HtmlDocument =
  val root_ : Lens<HtmlDocument, HtmlNode>

module HtmlNode =
  val nodes_ : System.Xml.XPath.XPathExpression
            -> Lens<HtmlNode, HtmlNode seq>

  val node_ : System.Xml.XPath.XPathExpression
            -> Prism<HtmlNode, HtmlNode>

  val innerText_ : Lens<HtmlNode, string>

module String =
  val int32_ : Epimorphism<string, int32>

  val realm_ : Epimorphism<string, HfVault.Realm.T>

module Seq =
  val enumerator_ : Lens<'α seq, 'α System.Collections.Generic.IEnumerator>
