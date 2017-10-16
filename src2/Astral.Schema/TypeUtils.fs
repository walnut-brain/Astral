namespace Astral.Schema
open System
open System.Runtime.CompilerServices


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module WellKnownType =
    let create<'t> name toItem (fromItem : DataItem -> Result<'t, DataItemConversionError>) =
        {
            dotNetType = typeof<'t>
            name = name
            toDataItem = 
                fun o ->
                    match o with
                    | :? 't as t -> toItem(t) |> Some
                    | _ -> None
            fromDataItem = 
                fun di -> (fromItem di) |> Result.map (fun o -> o :> obj) 
        } 
        
    let U8 = 
        let fto = PrimeItem.U8 >> Prime
        let ffrom p =
            match p with
            | Prime(U8(t)) -> Ok t
            | _ -> InvalidDataItem (sprintf "Awaited U8, but exists %A" p) |> Error  
        create<byte> "u8" fto ffrom
    let I8 =
        let fto = PrimeItem.I8 >> Prime
        let ffrom p =
            match p with
            | Prime(I8(t)) -> Ok t
            | _ -> InvalidDataItem (sprintf "Awaited I8, but exists %A" p) |> Error  
        create<sbyte> "i8" fto ffrom
        
    let U16 = 
        let fto = PrimeItem.U16 >> Prime
        let ffrom p =
            match p with
            | Prime(U16(t)) -> Ok t
            | _ -> InvalidDataItem (sprintf "Awaited U16, but exists %A" p) |> Error  
        create<uint16> "u16" fto ffrom
    let I16 =
        let fto = PrimeItem.I16 >> Prime
        let ffrom p =
            match p with
            | Prime(I16(t)) -> Ok t
            | _ -> InvalidDataItem (sprintf "Awaited I16, but exists %A" p) |> Error  
        create<int16> "i16" fto ffrom
        
    let U32 = 
        let fto = PrimeItem.U32 >> Prime
        let ffrom p =
            match p with
            | Prime(U32(t)) -> Ok t
            | _ -> InvalidDataItem (sprintf "Awaited U32, but exists %A" p) |> Error  
        create<uint32> "u32" fto ffrom
    let I32 =
        let fto = PrimeItem.I32 >> Prime
        let ffrom p =
            match p with
            | Prime(I32(t)) -> Ok t
            | _ -> InvalidDataItem (sprintf "Awaited I32, but exists %A" p) |> Error  
        create<int32> "i32" fto ffrom
        
    let U64 = 
        let fto = PrimeItem.U64 >> Prime
        let ffrom p =
            match p with
            | Prime(U64(t)) -> Ok t
            | _ -> InvalidDataItem (sprintf "Awaited U64, but exists %A" p) |> Error  
        create<uint64> "u64" fto ffrom
    let I64 =
        let fto = PrimeItem.I64 >> Prime
        let ffrom p =
            match p with
            | Prime(I64(t)) -> Ok t
            | _ -> InvalidDataItem (sprintf "Awaited I64, but exists %A" p) |> Error  
        create<int64> "i64" fto ffrom
        
    let F32 = 
        let fto = PrimeItem.F32 >> Prime
        let ffrom p =
            match p with
            | Prime(F32(t)) -> Ok t
            | _ -> InvalidDataItem (sprintf "Awaited F32, but exists %A" p) |> Error  
        create<float32> "f32" fto ffrom
    let F64 =
        let fto = PrimeItem.F64 >> Prime
        let ffrom p =
            match p with
            | Prime(F64(t)) -> Ok t
            | _ -> InvalidDataItem (sprintf "Awaited F64, but exists %A" p) |> Error  
        create<float> "f64" fto ffrom
                            
    let DT = 
        let fto = PrimeItem.DT >> Prime
        let ffrom p =
            match p with
            | Prime(DT(t)) -> Ok t
            | _ -> InvalidDataItem (sprintf "Awaited DT, but exists %A" p) |> Error  
        create<DateTime> "datetime" fto ffrom
    let DTO =
        let fto = PrimeItem.DTO >> Prime
        let ffrom p =
            match p with
            | Prime(DTO(t)) -> Ok t
            | _ -> InvalidDataItem (sprintf "Awaited DTO, but exists %A" p) |> Error  
        create<DateTimeOffset> "datetimeoffset" fto ffrom
        
    let String =
        let fto = PrimeItem.String >> Prime
        let ffrom p =
            match p with
            | Prime(String(t)) -> Ok t
            | _ -> InvalidDataItem (sprintf "Awaited String, but exists %A" p) |> Error  
        create<string> "string" fto ffrom
        
    let Uuid =
        let fto = PrimeItem.Uuid >> Prime
        let ffrom p =
            match p with
            | Prime(Uuid(t)) -> Ok t
            | _ -> InvalidDataItem (sprintf "Awaited Uuid, but exists %A" p) |> Error  
        create<Guid> "uuid" fto ffrom
        
    let TimeSpan =
        let fto = PrimeItem.TimeSpan >> Prime
        let ffrom p =
            match p with
            | Prime(TimeSpan(t)) -> Ok t
            | _ -> InvalidDataItem (sprintf "Awaited TimeSpan, but exists %A" p) |> Error  
        create<TimeSpan> "timespan" fto ffrom


type TypeSystem(types: DataType list) =
    let byType = Lazy<_>(fun () -> types |> List.collect (fun p -> match p.DotNetType with | Some t -> [t, p] | _ -> []) |> HashMap.create)
    let byName = Lazy<_>(fun () -> types |> List.map (fun p -> p.Name, p) |> Map.ofList)
    let byContract = Lazy<_>(fun () -> types |> List.map (match p -> p.Contract with | Some t -> [t, p] | _ -> []) |> Map.ofList) 
    member __.Types = types |> Seq.ofList
    member __.ByType = byType.Value   
    member __.ByName = byName.Value
    member __.ByContract = byContract.Value

module TypeSystem =
    type Builder =
        internal {
            system : TypeSystem ref  
        }
    let create() =
         let wk = 
             [ 
                 WellKnownType.U8 |> WellKnown; WellKnownType.I8 |> WellKnown; WellKnownType.U16 |> WellKnown; WellKnownType.I16 |> WellKnown
                 WellKnownType.U32 |> WellKnown; WellKnownType.I32 |> WellKnown; WellKnownType.U64 |> WellKnown; WellKnownType.I64 |> WellKnown
                 WellKnownType.F32 |> WellKnown; WellKnownType.F64 |> WellKnown; WellKnownType.DT |> WellKnown; WellKnownType.DTO |> WellKnown
                 WellKnownType.String |> WellKnown; WellKnownType.Uuid |> WellKnown; WellKnownType.TimeSpan |> WellKnown 
             ]
         { system = ref (TypeSystem wk) }
    let createRef 
    
     
     

           
       
type TypeSystemBuilder private (system) =
    let mutable system = system
    new () =
        let wk = 
            [ 
                WellKnownType.U8 |> WellKnown; WellKnownType.I8 |> WellKnown; WellKnownType.U16 |> WellKnown; WellKnownType.I16 |> WellKnown
                WellKnownType.U32 |> WellKnown; WellKnownType.I32 |> WellKnown; WellKnownType.U64 |> WellKnown; WellKnownType.I64 |> WellKnown
                WellKnownType.F32 |> WellKnown; WellKnownType.F64 |> WellKnown; WellKnownType.DT |> WellKnown; WellKnownType.DTO |> WellKnown
                WellKnownType.String |> WellKnown; WellKnownType.Uuid |> WellKnown; WellKnownType.TimeSpan |> WellKnown 
            ] 
        TypeSystemBuilder(TypeSystem(wk))
    member 
  
            