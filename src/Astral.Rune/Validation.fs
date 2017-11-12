module Astral.Rune.Validation
open System
open Ast


let isOrdinalLiteralAssignableTo ot lit =
    match ot with
    | U8 ->
        match lit with
        | U8Literal _ -> true                            | I8Literal i8 -> i8 >= 0y
        | U16Literal u16 -> u16 <= uint16(Byte.MaxValue) | I16Literal i16 -> i16 >= 0s && i16 <= int16(Byte.MaxValue)
        | U32Literal u32 -> u32 <= uint32(Byte.MaxValue) | I32Literal i32 -> i32 >= 0 && i32 <= int32(Byte.MaxValue)
        | U64Literal u64 -> u64 <= uint64(Byte.MaxValue) | I64Literal i64 -> i64 >= 0L && i64 <= int64(Byte.MaxValue)
    | I8 -> 
        match lit with
        | U8Literal u8 -> u8 <= uint8(SByte.MaxValue)     | I8Literal i8 -> true
        | U16Literal u16 -> u16 <= uint16(SByte.MaxValue) | I16Literal i16 -> i16 >= int16(SByte.MinValue) && i16 <= int16(SByte.MaxValue)
        | U32Literal u32 -> u32 <= uint32(SByte.MaxValue) | I32Literal i32 -> i32 >= int32(SByte.MinValue) && i32 <= int32(SByte.MaxValue)
        | U64Literal u64 -> u64 <= uint64(SByte.MaxValue) | I64Literal i64 -> i64 >= int64(SByte.MinValue) && i64 <= int64(SByte.MaxValue)
    | U16 ->
        match lit with
        | U8Literal _ -> true                              | I8Literal i8 -> i8 >= 0y
        | U16Literal u16 -> true                           | I16Literal i16 -> i16 >= 0s 
        | U32Literal u32 -> u32 <= uint32(UInt16.MaxValue) | I32Literal i32 -> i32 >= 0 && i32 <= int32(UInt16.MaxValue)
        | U64Literal u64 -> u64 <= uint64(UInt16.MaxValue) | I64Literal i64 -> i64 >= 0L && i64 <= int64(UInt16.MaxValue)
    | I16 -> 
        match lit with
        | U8Literal u8 -> true                            | I8Literal i8 -> true
        | U16Literal u16 -> u16 <= uint16(Int16.MaxValue) | I16Literal i16 -> true
        | U32Literal u32 -> u32 <= uint32(Int16.MaxValue) | I32Literal i32 -> i32 >= int32(Int16.MinValue) && i32 <= int32(Int16.MaxValue)
        | U64Literal u64 -> u64 <= uint64(Int16.MaxValue) | I64Literal i64 -> i64 >= int64(Int16.MinValue) && i64 <= int64(Int16.MaxValue)
    | U32 ->
        match lit with
        | U8Literal _ -> true                              | I8Literal i8 -> i8 >= 0y
        | U16Literal u16 -> true                           | I16Literal i16 -> i16 >= 0s 
        | U32Literal u32 -> true                           | I32Literal i32 -> i32 >= 0
        | U64Literal u64 -> u64 <= uint64(UInt32.MaxValue) | I64Literal i64 -> i64 >= 0L && i64 <= int64(UInt32.MaxValue)
    | I32 -> 
        match lit with
        | U8Literal u8 -> true                            | I8Literal i8 -> true
        | U16Literal u16 -> true                          | I16Literal i16 -> true
        | U32Literal u32 -> u32 <= uint32(Int32.MaxValue) | I32Literal i32 -> true
        | U64Literal u64 -> u64 <= uint64(Int32.MaxValue) | I64Literal i64 -> i64 >= int64(Int32.MinValue) && i64 <= int64(Int32.MaxValue)
    | U64 ->
        match lit with
        | U8Literal _ -> true                              | I8Literal i8 -> i8 >= 0y
        | U16Literal u16 -> true                           | I16Literal i16 -> i16 >= 0s 
        | U32Literal u32 -> true                           | I32Literal i32 -> i32 >= 0
        | U64Literal u64 -> true                           | I64Literal i64 -> i64 >= 0L
    | I64 -> 
        match lit with
        | U8Literal u8 -> true                            | I8Literal i8 -> true
        | U16Literal u16 -> true                          | I16Literal i16 -> true
        | U32Literal u32 -> true                          | I32Literal i32 -> true
        | U64Literal u64 -> u64 <= uint64(Int64.MaxValue) | I64Literal i64 -> true 

let private extractTypeNames refs =
    let rec foldRefs ref acc =
        match ref with
        | NamedType id -> id :: acc
        | ArrayType r1 -> foldRefs r1 acc
        | MayBeType r2 -> foldRefs r2 acc
        | _ -> acc
    refs |> List.fold (fun s r -> (foldRefs r []) @ s) []
    
let validateEnumType ot vl  =
    let invalid = vl |> Map.toSeq |> Seq.tryFind (fun (id, v) -> isOrdinalLiteralAssignableTo ot v |> not)
    match invalid with
    | None -> Ok()
    | Some (id, v) -> sprintf "Value %A is not assignable to type %A in field %A" v ot id |> Error   

module private TypeChecker =
    type Checker = private {
        Known : Set<Identifier>
        Unknown : Set<Identifier>
    } 
    
    let create () = { Known = Set.empty; Unknown = Set.empty }
    
    let markKnown id checker =
        { Unknown = checker.Unknown |> Set.remove id; Known = checker.Known |> Set.add id }
    let addUndefined checker id  =
        if checker.Known |> Set.contains id then
            checker
        else
            { checker with Unknown = checker.Unknown |> Set.add id }
    let toErrorList checker =
        checker.Unknown |> Seq.map (fun id -> sprintf "Unknown type %A" id) |> Seq.toList

let validateSchema schema =
    let errors = ResizeArray();
    let validateTypes types =
        let foldType (checker, errors) (id, tip)  =
            match tip with
            | EnumType (fl, ot, vl) -> 
                  TypeChecker.markKnown id checker,  
                  match validateEnumType ot vl with
                  | Ok _ -> errors
                  | Error er -> er :: errors    
            | MapType mt
            | OneOfType mt ->
                let chk1 = TypeChecker.markKnown id checker
                let references =
                    match mt with
                    | Indexed it -> Map.toList it |> List.map snd
                    | Named nt -> Map.toList nt |> List.map snd
                references 
                    |> extractTypeNames
                    |> List.fold TypeChecker.addUndefined chk1, errors      
        types |> Map.toSeq |> Seq.fold foldType (TypeChecker.create(), [])
    let validateEndpoints checker eps =
        let validate chk ep =
            seq { 
                match ep with
                | Event {EventType = et} ->
                    yield et
                | Call cl ->
                    
                        yield cl.Result
                        match cl.Arguments with
                        | Single s -> yield s
                        | Multiple mp -> yield! (mp |> Map.toSeq |> Seq.map snd) 
            } |> Seq.toList 
              |> extractTypeNames
              |> List.fold TypeChecker.addUndefined chk  
        eps |> List.fold validate checker
    let (checker, errors) = validateTypes schema.Types 
    let checker = schema.Services 
                    |> Map.toList 
                    |> List.map snd 
                    |> List.collect (fun (p, e) -> p |> Map.toList |> List.map snd |> List.map fst)
                    |> validateEndpoints checker
    let errors = TypeChecker.toErrorList checker |> List.append errors
    match errors with
    | [] -> Ok()
    | _ -> Error errors
