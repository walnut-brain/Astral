namespace Astral.Schema
open System.Runtime.CompilerServices

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module WellKnownType =

    [<Struct>]
    type internal SU8 =
        interface SDataItem<byte> with
            member __.ToDataItem p = U8 p |> Ordinal |> WellKnown 
            member __.FromDataItem p =
                match p with
                | WellKnown(Ordinal(U8(t))) -> Ok t
                | _ -> InvalidDataItem (sprintf "Awaited U8, but exists %A" p) |> Error
                
    [<Struct>]
    type internal SI8 =
        interface SDataItem<sbyte> with
            member __.ToDataItem p = I8 p |> Ordinal |> WellKnown 
            member __.FromDataItem p =
                match p with
                | WellKnown(Ordinal(I8(t))) -> Ok t
                | _ -> InvalidDataItem (sprintf "Awaited U8, but exists %A" p) |> Error
                  

    let addType (wt: WellKnownType<'t, 's>) wtc =
            { wtc with fromType = wtc.fromType =?> (fun t -> if t = typeof<'t> then Some(wt :> WellKnownType) else None) }
    let createDictionary () =
        
        { fromType = fun _ -> None } 
            |> addType (WellKnownType<byte, SU8> "u8")
            |> addType (WellKnownType<sbyte, SI8> "i8")
    
    let tryGetWellKnownType wtc typ =
        wtc.fromType typ