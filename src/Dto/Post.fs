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

namespace HfVault
#nowarn "62"
#light "off"

namespace HfVault.Dto
module Post = begin
  type DbRepr = { id : int
                ; author : Author.DbRepr
                ; createdAt : System.DateTime
                ; message : string
                ; thread : int
                }

  type T = T of (int * Author.T * System.DateTime * string * int)

  let toDb (T (id, author, createdAt, message, thread)) =
    { id=id
    ; author=author |> Author.toDb
    ; createdAt=createdAt
    ; message=message
    ; thread=thread
    }
end

namespace HfVault.Domain
module Post = begin
  open System
  open HtmlAgilityPack

  type T = { id : int
           ; author : Author.T
           ; createdAt : DateTime
           ; message : HtmlNode
           ; thread : int
           }

  let new_ = { id=0
             ; author=Author.new_
             ; createdAt=Unchecked.defaultof<_>
             ; message=Unchecked.defaultof<_>
             ; thread=0
             }

  let id_ = (fun {id=i} -> i), (fun i xx -> {xx with id=i})
  let author_ = (fun {author=a} -> a), (fun a xx -> {xx with author=a})
  let createdAt_ = (fun {createdAt=c} -> c), (fun c xx -> {xx with createdAt=c})
  let message_ : Aether.Lens<_, _> = (fun {message=m} -> m), (fun m xx -> {xx with message=m})
  let thread_ = (fun {thread=t} -> t), (fun t xx -> {xx with thread=t})

  let dto_ =
  ( ( fun (HfVault.Dto.Post.T (id, author, createdAt, message, thread)) ->
        { id=id
        ; author=author |> (fst Author.dto_)
        ; createdAt=createdAt
        ; message=HtmlNode.CreateNode(message)
        ; thread=thread
        }
    )
  , ( fun xx ->
        HfVault.Dto.Post.T
        ( xx.id
        , xx.author |> (snd Author.dto_)
        , xx.createdAt
        , xx.message.OuterHtml
        , xx.thread
        )
    )
  )
end
