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
module Thread = begin
  type DbRepr = { id : int
                ; author : Author.DbRepr
                ; createdAt : System.DateTime
                ; updatedAt : System.DateTime
                ; theme : Theme.DbRepr
                ; name : string
                ; ``open`` : bool
                ; sticky : bool
                }

  type T = { id : int
           ; author : Author.T
           ; createdAt : System.DateTime
           ; updatedAt : System.DateTime
           ; theme : Theme.T
           ; name : string
           ; ``open`` : bool
           ; sticky : bool
           }

  let toDb xx =
    { DbRepr.id=xx.id
    ; DbRepr.author=xx.author |> Author.toDb
    ; DbRepr.createdAt=xx.createdAt
    ; DbRepr.updatedAt=xx.updatedAt
    ; DbRepr.theme=xx.theme |> Theme.toDb
    ; DbRepr.name=xx.name
    ; DbRepr.``open``=xx.``open``
    ; DbRepr.sticky=xx.sticky
    }
end

namespace HfVault.Domain
module Thread = begin
  type T = { id : int
           ; author : Author.T
           ; createdAt : System.DateTime
           ; updatedAt : System.DateTime
           ; theme : Theme.T
           ; name : string
           ; ``open`` : bool
           ; sticky : bool
           }

  let new_ = { id=0
             ; author=Author.new_
             ; createdAt=Unchecked.defaultof<_>
             ; updatedAt=Unchecked.defaultof<_>
             ; theme=Theme.new_
             ; name=null
             ; ``open``=false
             ; sticky=false
             }

  let id_ = (fun {id=i} -> i), (fun i xx -> {xx with id=i})
  let author_ = (fun {author=a} -> a), (fun a xx -> {xx with author=a})
  let createdAt_ = (fun {createdAt=c} -> c), (fun c xx -> {xx with createdAt=c})
  let updatedAt_ = (fun {updatedAt=u} -> u), (fun u xx -> {xx with updatedAt=u})
  let theme_ = (fun {theme=t} -> t), (fun t xx -> {xx with theme=t})
  let name_ = (fun {name=n} -> n), (fun n xx -> {xx with name=n})
  let open_ = (fun {``open``=o} -> o), (fun o xx -> {xx with ``open``=o})
  let sticky_ = (fun {sticky=s} -> s), (fun s xx -> {xx with sticky=s})

  let dto_ =
  ( ( fun (dto : HfVault.Dto.Thread.T) ->
        { id=dto.id
        ; author=dto.author |> (fst Author.dto_)
        ; createdAt=dto.createdAt
        ; updatedAt=dto.updatedAt
        ; theme=dto.theme |> (fst Theme.dto_)
        ; name=dto.name
        ; ``open``=dto.``open``
        ; sticky=dto.sticky
        }
    )
  , ( fun xx ->
        { HfVault.Dto.Thread.T.id=xx.id
        ; HfVault.Dto.Thread.T.author=xx.author |> (snd Author.dto_)
        ; HfVault.Dto.Thread.T.createdAt=xx.createdAt
        ; HfVault.Dto.Thread.T.updatedAt=xx.updatedAt
        ; HfVault.Dto.Thread.T.theme=xx.theme |> (snd Theme.dto_)
        ; HfVault.Dto.Thread.T.name=xx.name
        ; HfVault.Dto.Thread.T.``open``=xx.``open``
        ; HfVault.Dto.Thread.T.sticky=xx.sticky
        }
    )
  )
end
