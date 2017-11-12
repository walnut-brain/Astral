module Astral.Rune.Markup
open System
open System.Reflection
open Ast

let Core = QualifiedIdentifier.Create("Core")

[<AbstractClass>]
type ParameterAttribute() =
    inherit Attribute()
    abstract ExtensionName : QualifiedIdentifier with get 

type ISchemaParameter =
    abstract Apply : Parameters -> Parameters 

type IServiceParameter =
    abstract Apply : Parameters -> Parameters

type IEndpointParameter =
    abstract Apply : Parameters -> Parameters
 
let private contentTypeId = QualifiedIdentifier.Create("ContentType");

[<AttributeUsage(AttributeTargets.Class ||| AttributeTargets.Interface ||| AttributeTargets.Property ||| AttributeTargets.Method)>] 
type ContentTypeAttribute(contentType) =
    inherit ParameterAttribute()
    override __.ExtensionName = Core
    member private __.Apply p =
        let (Parameters m) = p
        m |> Map.add contentTypeId (contentType |> StringLiteral |> BasicLiteral) |> Parameters
    interface ISchemaParameter with
        member __.Apply p = __.Apply p
    interface IServiceParameter with
        member __.Apply p = __.Apply p
    interface IEndpointParameter with
        member __.Apply p = __.Apply p
    
let private createServiceName (name: string) =
    if name.Length > 1 then
        if name.StartsWith("I") && Char.IsUpper(name.[1]) then
            Identifier.Create(name.Substring(1))
        else
            Identifier.Create(name)
    else
        Identifier.Create(name)

module TypeManager = 
    type Manager = private | Manager


type ProcessServiceError =
    | ServiceAlreadyExists of string

let findEndpoints () =

let processService (intf : TypeInfo) typeManager schema =
    

    let name = createServiceName intf.Name
    if schema.Services |> Map.containsKey name then
        sprintf "Service with name %A already exists" name |> ServiceAlreadyExists |> Error
    else
