﻿module FSharp.Monad.Monoid

open System
open System.Collections.Generic

/// The monoid.
/// The monoid implementation comes from Matthew Podwysocki's http://codebetter.com/blogs/matthew.podwysocki/archive/2010/02/01/a-kick-in-the-monads-writer-edition.aspx.
type IMonoid<'a> =
  abstract member mempty  : unit -> 'a
  abstract member mappend : 'a * 'a -> 'a

type ListMonoid<'a>() =
  interface IMonoid<'a list> with
    member this.mempty() = []
    member this.mappend(a,b) = a @ b

type MonoidAssociations private() =
  static let associations = new Dictionary<Type, obj>()
  static member Add<'a>(monoid : IMonoid<'a>) = associations.Add(typeof<'a>, monoid)
  static member Get<'a>() =
    match associations.TryGetValue(typeof<'a>) with
    | true, assoc -> assoc :?> IMonoid<'a>
    | false, _    -> failwithf "No IMonoid defined for %O" <| typeof<'a>

MonoidAssociations.Add(new ListMonoid<string>())
let mempty<'a> = MonoidAssociations.Get<'a>().mempty
let mappend<'a> a b = MonoidAssociations.Get<'a>().mappend(a, b)

let inline mreturn builder x =
  (^a: (member Return: 'b -> 'c) (builder, x))

let inline bind builder m f =
  (^a: (member Bind: 'd * ('e -> 'c) -> 'c) (builder, m, f))

let inline lift builder f m =
  bind builder m (mreturn builder)
