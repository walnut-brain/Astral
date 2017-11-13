namespace rec Astral.Rune.Dom
open Astral.Rune.Ast

type Schema(declaration) =
    new(nameSpace, version) =
        Schema(
            {
                Namespace = nameSpace
                Version = version
                Types = Map.empty
                Services = Map.empty
                Extensions = Set.empty
            })
    member __.Namespace = declaration.Namespace
    member __.Version = declaration.Version
    member __.SetNamespace nameSpace =
        Schema({ declaration with Namespace = nameSpace })
    member __.SetVersion version =
        Schema({ declaration with Version = version })