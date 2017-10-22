namespace Astral.Schema.Ast
open System

type IReplacer =
    abstract ReplaceNode : oldNode : IReplacer * newNode : IReplacer -> IReplacer

module Replacer =

    let replaceSingle<'b when 'b :> IReplacer>  (oldNode : IReplacer)  newNode  (curVal : 'b) =
        if obj.Equals(curVal, oldNode) then
            match newNode |> box with
            | :? 'b as bv ->  bv
            | _ -> curVal.ReplaceNode(oldNode, newNode) :?> 'b
        else
            curVal.ReplaceNode(oldNode, newNode) :?> 'b
        
    let replaceAll oldNode newNode lst =
        lst |> List.map (replaceSingle oldNode newNode)

    let replaceNode<'a when 'a :> IReplacer> oldNode newNode (el : 'a) =
        el.ReplaceNode(oldNode, newNode) :?> 'a
            



type IGreenNode =
    inherit IReplacer

type InvalidIdentifier =
    | NullOrWhitespace
    | InvalidStartCharacter of char
    | InvalidCharacter of char list

module internal IdentifierUtils =
    let private letters = ['a'..'z'] @ ['A'..'Z']
    let private letterOrDigitOrUnderscore = letters @ ['0'..'9'] @ ['_']  
    let isAsciiLetter ch = letters |> List.contains ch 
    let isAsciiLetterOrDigitOrUnderscore ch = letterOrDigitOrUnderscore |> List.contains ch
    
    let (|IsAsciiLetter|_|) ch  = 
        if isAsciiLetter ch then Some() else None

    let validate name =
        if (String.IsNullOrWhiteSpace name) then Error(NullOrWhitespace)
        else
            match name.[0] with
            | IsAsciiLetter _ 
            | '_' ->
                let bad = name.Substring(1) |> Seq.filter (isAsciiLetterOrDigitOrUnderscore >> not) |> Seq.toList
                match bad with
                | [] -> Ok()
                | _ -> InvalidCharacter bad |> Error
            | ch -> InvalidStartCharacter ch |> Error

type Identifier = 
    internal 
        | Identifier of string
    static member Create name =
        match IdentifierUtils.validate name with
        | Error(NullOrWhitespace) ->
            raise (ArgumentException "Null or empty string")
        | Error(InvalidStartCharacter ch) ->
            raise (ArgumentException (sprintf "Invalid start character %A" ch))
        | Error(InvalidCharacter chl) ->
            raise (ArgumentException (sprintf "Invalid characters %A" chl))
        | Ok() -> Identifier name
    static member TryCreate name =
        IdentifierUtils.validate name |> Result.map (fun () -> Identifier name)
    override __.ToString () =
        let (Identifier name) = __ in name






type NamespaceName = Identifier list 
type QualifiedIdentifier = NamespaceName * Identifier

type OrdinalType =
    | U8 | I8 | U16 | I16 | U32 | I32 | U64 | I64
    member __.Name =
        match __ with
        | U8  -> "u8" | I8  -> "i8"  | U16 -> "u16" | I16 -> "i16" 
        | U32 -> "u32"| I32 -> "i32" | U64 -> "u64" | I64 -> "i64"
    member __.Type =
        match __ with
        | U8  -> typeof<byte>   | I8  -> typeof<sbyte>  | U16 -> typeof<uint16> | I16 -> typeof<int16> 
        | U32 -> typeof<uint32> | I32 -> typeof<int32>  | U64 -> typeof<uint64> | I64 -> typeof<int64>
    interface IReplacer with
        member __.ReplaceNode (_, _) = __ :> IReplacer
    interface IGreenNode

type BasicType =
    | Ordinal of OrdinalType
    | F32 | F64 | String | Uuid | DT | DTO | TS
    member __.Name =
      match __ with
      | Ordinal ord -> ord.Name | F32 -> "f32"      | F64 -> "f64"       | String -> "string"
      | Uuid        -> "uuid"   | DT  -> "datetime" | DTO -> "datetime2" | TS     -> "timespan"  
    interface IReplacer with
        member __.ReplaceNode (_, _) = __ :> IReplacer
    interface IGreenNode