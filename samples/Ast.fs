module Rune.Ast

open System
open System.Text.RegularExpressions
open System.Runtime.InteropServices

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
    | Complex of QualifiedIdentifier * Identifier
    
    
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

type ComplexTypeDescription =
    | Indexed of Map<NameAndIndex, TypeReference>
    | Named of Map<Identifier, TypeReference>


type TypeDescription =
    | EnumType of IsFlag : bool * Based : OrdinalType * Values : Map<Identifier, OrdinalLiteral>
    | MapType of ComplexTypeDescription
    | OneOfType of ComplexTypeDescription

type Extensions = Map<QualifiedIdentifier, Literal>

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

type ServiceDescription = Map<Identifier, EndpointDescription * Extensions> * Extensions

type SemanticVersion = Version of string

type SchemaDeclaration =
    {
        Namespace : QualifiedIdentifier
        Version : SemanticVersion
        Types : Map<Identifier, TypeDescription>
        Services : Map<Identifier, ServiceDescription>        
    }