module Astral.Rune.Ast

open System
open System.Text.RegularExpressions
open System.Runtime.InteropServices
open System.Collections.Generic


let private identifierRegEx = Regex("[_a-zA-Z][_a-zA-Z0-9]*")

type Identifier = 
        private | Identifier of string 
        static member TryCreate (str) =
            if identifierRegEx.IsMatch(str) then Identifier str |> Ok else Error "Invalid identifier"
        static member TryMake (str, [<Out>] id : Identifier byref)  = 
            match Identifier.TryCreate(str) with
            | Ok s -> id <- s; true
            | _ -> id <- Unchecked.defaultof<Identifier>; false
        static member Create s =
            match Identifier.TryCreate s with
            | Ok s -> s
            | Error e -> raise (ArgumentException e)
    
type QualifiedIdentifier =
    | Simple of Identifier
    | Complex of Identifier * QualifiedIdentifier
    static member TryCreate (ns: string) =
        let rec fromList lst =
            match lst with
            | [] -> Error("empty name")
            | [id] -> Simple id |> Ok
            | id :: tail -> fromList tail |> Result.map (fun p -> Complex(id, p))
        let folder state el =     
            match state with
            | Ok p ->
                match el with
                | Ok e -> Ok(e :: p)
                | Error s -> Error s
            | Error s ->
                match el with
                | Error s1 -> Error (s1 + Environment.NewLine + s)
                | _ -> state
                    
        let splitted = 
            ns.Split(':') 
                |> Array.toList 
                |> List.map Identifier.TryCreate
                |> List.rev
                |> List.fold folder (Ok []) 
        match splitted with
        | Error s -> Error s
        | Ok sp -> fromList sp 
    static member TryMake (str, [<Out>] id : QualifiedIdentifier byref) =
        match QualifiedIdentifier.TryCreate(str) with
        | Ok s -> id <- s; true
        | _ -> id <- Unchecked.defaultof<QualifiedIdentifier>; false
    static member Create s =
        match QualifiedIdentifier.TryCreate s with
        | Ok s -> s
        | Error e -> raise (ArgumentException e)
        
    
    
type OrdinalLiteral =
    | U8Literal of byte
    | I8Literal of sbyte
    | U16Literal of uint16
    | I16Literal of int16
    | U32Literal of uint32
    | I32Literal of int32
    | U64Literal of uint64
    | I64Literal of int64
            
type BasicLiteral =
    | OrdinalLiteral of OrdinalLiteral
    | F32Literal of float32
    | F64Literal of float
    | StringLiteral of string
    | UuidLiteral of Guid
    | BoolLiteral of bool
    | DTLiteral of DateTime
    | DTOLiteral of DateTimeOffset
    | TSLiteral of TimeSpan
    | NoneLiteral

type Literal =
    | IdentifierLiteral of QualifiedIdentifier
    | BasicLiteral of BasicLiteral 
    | ArrayLiteral of Literal list
    | MapLiteral of Map<Identifier, Literal>

type OrdinalType =
    | U8 | I8 | U16 | I16 | U32 | I32 | U64 | I64
    

type BasicType =
    | Ordinal of OrdinalType
    | F32 | F64 | String | Uuid | DT | DTO | TS | Bool

[<CustomEquality>]
[<CustomComparison>]
type NameAndIndex =
    {
        Name : Identifier
        Index : int
    }
    override __.Equals(other) =
        match other with
        | :? NameAndIndex as y -> __.Name = y.Name || __.Index = y.Index
        | _ -> false
    override __.GetHashCode() = __.Index.GetHashCode()
    interface IEquatable<NameAndIndex> with
        member __.Equals other =
            __.Name = other.Name || __.Index = other.Index
    interface IComparable<NameAndIndex> with
        member __.CompareTo other =
            if __.Name = other.Name || __.Index = other.Index 
                then 0
                else __.Index.CompareTo(other.Index)
    interface IComparable with
        member __.CompareTo other =
            match other with
            | :? NameAndIndex as y -> (__ :> IComparable<NameAndIndex>).CompareTo(y)
            | _ -> invalidArg "other" "cannot compare values of different types"


type TypeReference =
    | UnitType
    | BasicType of BasicType
    | ArrayType of TypeReference
    | MayBeType of TypeReference
    | NamedType of Identifier
    | TupleType of TypeReference list


type Modifiers = Map<QualifiedIdentifier, Literal>

type FieldedTypeDescription =
    | Indexed of Map<NameAndIndex, TypeReference * Modifiers>
    | Named of Map<Identifier, TypeReference * Modifiers>

type EnumTypeDescription =
    {
        IsFlag : bool
        Based : OrdinalType
        Values : Map<Identifier, OrdinalLiteral>
    }

type ComplexTypeDescription =
    | EnumType of EnumTypeDescription
    | MapType of FieldedTypeDescription
    | OneOfType of FieldedTypeDescription

type TypeDescription =
    {
        Body: ComplexTypeDescription
        Modifiers : Modifiers
    }

type EventDescription =
    {
        EventType : TypeReference
    } 
    
type CallArguments =
    | Single of TypeReference
    | Multiple of Map<Identifier, TypeReference> 

type CallDescription =
    {
        Arguments : CallArguments
        Result : TypeReference
    }

type EndpointDescription =
    | Event of EventDescription
    | Call of CallDescription

type ServiceDescription = Map<Identifier, EndpointDescription * Modifiers> * Modifiers

type SemanticVersion = Version of string

type SchemaDeclaration =
    {
        Namespace : QualifiedIdentifier
        Extensions : Set<QualifiedIdentifier>
        Version : SemanticVersion
        Types : Map<Identifier, TypeDescription>
        Services : Map<Identifier, ServiceDescription>        
    }
    
    
    


        
      
    
    

    
      