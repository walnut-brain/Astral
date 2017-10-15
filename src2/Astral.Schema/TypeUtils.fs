namespace Astral.Schema
open System
open System.Runtime.CompilerServices


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module WellKnownType =
    [<Struct>]
    type private SU8 =
        interface SDataItem<byte> with
            member __.ToDataItem p = U8 p |> WellKnown 
            member __.FromDataItem p =
                match p with
                | WellKnown(U8(t)) -> Ok t
                | _ -> InvalidDataItem (sprintf "Awaited U8, but exists %A" p) |> Error
                    
    [<Struct>]
    type private SI8 =
        interface SDataItem<sbyte> with
            member __.ToDataItem p = I8 p |> WellKnown 
            member __.FromDataItem p =
                match p with
                | WellKnown(I8(t)) -> Ok t
                | _ -> InvalidDataItem (sprintf "Awaited I8, but exists %A" p) |> Error
    
    [<Struct>]
    type private SU16 =
        interface SDataItem<uint16> with
            member __.ToDataItem p = U16 p |> WellKnown 
            member __.FromDataItem p =
                match p with
                | WellKnown(U16(t)) -> Ok t
                | _ -> InvalidDataItem (sprintf "Awaited U16, but exists %A" p) |> Error
                    
    [<Struct>]
    type private SI16 =
        interface SDataItem<int16> with
            member __.ToDataItem p = I16 p |> WellKnown 
            member __.FromDataItem p =
                match p with
                | WellKnown(I16(t)) -> Ok t
                | _ -> InvalidDataItem (sprintf "Awaited I16, but exists %A" p) |> Error

    [<Struct>]
    type private SU32 =
        interface SDataItem<uint32> with
            member __.ToDataItem p = U32 p |> WellKnown 
            member __.FromDataItem p =
                match p with
                | WellKnown(U32(t)) -> Ok t
                | _ -> InvalidDataItem (sprintf "Awaited U32, but exists %A" p) |> Error
                    
    [<Struct>]
    type private SI32 =
        interface SDataItem<int32> with
            member __.ToDataItem p = I32 p |> WellKnown 
            member __.FromDataItem p =
                match p with
                | WellKnown(I32(t)) -> Ok t
                | _ -> InvalidDataItem (sprintf "Awaited I32, but exists %A" p) |> Error
    
    [<Struct>]
    type private SU64 =
        interface SDataItem<uint64> with
            member __.ToDataItem p = U64 p |> WellKnown 
            member __.FromDataItem p =
                match p with
                | WellKnown(U64(t)) -> Ok t
                | _ -> InvalidDataItem (sprintf "Awaited U64, but exists %A" p) |> Error
                    
    [<Struct>]
    type private SI64 =
        interface SDataItem<int64> with
            member __.ToDataItem p = I64 p |> WellKnown 
            member __.FromDataItem p =
                match p with
                | WellKnown(I64(t)) -> Ok t
                | _ -> InvalidDataItem (sprintf "Awaited I64, but exists %A" p) |> Error
                
    [<Struct>]
    type private SF32 =
        interface SDataItem<float32> with
            member __.ToDataItem p = F32 p |> WellKnown 
            member __.FromDataItem p =
                match p with
                | WellKnown(F32(t)) -> Ok t
                | _ -> InvalidDataItem (sprintf "Awaited F32, but exists %A" p) |> Error
                    
    [<Struct>]
    type private SF64 =
        interface SDataItem<float> with
            member __.ToDataItem p = F64 p |> WellKnown 
            member __.FromDataItem p =
                match p with
                | WellKnown(F64(t)) -> Ok t
                | _ -> InvalidDataItem (sprintf "Awaited F64, but exists %A" p) |> Error
    
    [<Struct>]
    type private SDT =
        interface SDataItem<DateTime> with
            member __.ToDataItem p = DT p |> WellKnown 
            member __.FromDataItem p =
                match p with
                | WellKnown(DT(t)) -> Ok t
                | _ -> InvalidDataItem (sprintf "Awaited DT, but exists %A" p) |> Error
                    
    [<Struct>]
    type private SDTO =
        interface SDataItem<DateTimeOffset> with
            member __.ToDataItem p = DTO p |> WellKnown 
            member __.FromDataItem p =
                match p with
                | WellKnown(DTO(t)) -> Ok t
                | _ -> InvalidDataItem (sprintf "Awaited DTO, but exists %A" p) |> Error
                
    [<Struct>]
    type private SString =
        interface SDataItem<string> with
            member __.ToDataItem p = WellKnownItem.String p |> WellKnown 
            member __.FromDataItem p =
                match p with
                | WellKnown(String(t)) -> Ok t
                | _ -> InvalidDataItem (sprintf "Awaited String, but exists %A" p) |> Error
    
    [<Struct>]
    type private SUuid =
        interface SDataItem<Guid> with
            member __.ToDataItem p = Uuid p |> WellKnown 
            member __.FromDataItem p =
                match p with
                | WellKnown(Uuid(t)) -> Ok t
                | _ -> InvalidDataItem (sprintf "Awaited Uuid, but exists %A" p) |> Error
                    
    [<Struct>]
    type private STimeSpan =
        interface SDataItem<TimeSpan> with
            member __.ToDataItem p = WellKnownItem.TimeSpan p |> WellKnown 
            member __.FromDataItem p =
                match p with
                | WellKnown(TimeSpan(t)) -> Ok t
                | _ -> InvalidDataItem (sprintf "Awaited TimeSpan, but exists %A" p) |> Error                
                
     

    
                  

    let addType (wt: WellKnownType<'t, 's>) wtc =
            { wtc with fromType = wtc.fromType =?> (fun t -> if t = typeof<'t> then Some(wt :> WellKnownType) else None) }
    let createDictionary () =
        
        { fromType = fun _ -> None } 
            |> addType (WellKnownType<byte, SU8> "u8")
            |> addType (WellKnownType<sbyte, SI8> "i8")
            |> addType (WellKnownType<uint16, SU16> "u16")
            |> addType (WellKnownType<int16, SI16> "i16")
            |> addType (WellKnownType<uint32, SU32> "u32")
            |> addType (WellKnownType<int32, SI32> "i32")
            |> addType (WellKnownType<uint64, SU64> "u64")
            |> addType (WellKnownType<int64, SI64> "i64")
            |> addType (WellKnownType<float32, SF32> "f32")
            |> addType (WellKnownType<float, SF64> "f64")
            |> addType (WellKnownType<DateTime, SDT> "datetime")
            |> addType (WellKnownType<DateTimeOffset, SDTO> "datetimeoffset")
            |> addType (WellKnownType<string, SString> "string")
            |> addType (WellKnownType<Guid, SUuid> "uuid")
            |> addType (WellKnownType<TimeSpan, STimeSpan> "timespan")
    
    let tryGetWellKnownType wtc typ =
        wtc.fromType typ