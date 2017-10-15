namespace rec Astral.Schema
open System
open System.Runtime.CompilerServices
open System.Net.Mime
open System.Linq


     

[<AutoOpen>]
module internal OptionF =
    let orElse f1 f2 =
        fun p -> f1 p |> Option.orElseWith (fun () -> f2 p) 
    let map f1 f2 =
        fun p -> f1 p |> Option.map f2   
        
    let (=?>) = orElse
    let (->>) = map
      


module internal Utils =
    let findFastInPairList list =
       let dict = list |> (fun p -> Enumerable.ToDictionary<_, _, _>(p, Func<_, _>(fst), Func<_, _>(snd)))
       fun typ -> 
            match dict.TryGetValue(typ) with
            | (true, v) -> Some v
            | _ -> None    
         
     
        
(*
type OrdinalType = 
    | U8 | I8 | U16 | I16 | U32 | I32 | U64 | I64
    member __.Name =
        match __ with
        | U8  -> TypeName "u8"
        | I8  -> TypeName "i8"
        | U16 -> TypeName "u16"
        | I16 -> TypeName "i16"
        | U32 -> TypeName "u32"
        | I32 -> TypeName "i32"
        | U64 -> TypeName "u64"
        | I64 -> TypeName "i64"
    member __.DotNetType =
        match __ with
        | U8  -> typeof<byte> |> Some
        | I8  -> typeof<sbyte> |> Some
        | U16 -> typeof<uint16> |> Some
        | I16 -> typeof<int16> |> Some
        | U32 -> typeof<uint32> |> Some
        | I32 -> typeof<int32> |> Some
        | U64 -> typeof<uint64> |> Some
        | I64 -> typeof<int64> |> Some
    static member TryResolve =
        Utils.findFastInPairList 
            [
                typeof<byte>,   U8
                typeof<sbyte>,  I8
                typeof<uint16>, U16
                typeof<int16>,  I16
                typeof<uint32>, U32
                typeof<int32>,  I32
                typeof<uint64>, U64
                typeof<int64>,  I64
            ] 
                
            
          
type RelationalType = 
    | F32 | F64
    member __.Name =
        match __ with
        | F32 -> TypeName "f32"
        | F64 -> TypeName "f64"
    member __.DotNetType = 
        match __ with
        | F32 -> typeof<float32> |> Some
        | F64 -> typeof<float> |> Some
    static member TryResolve =
        Utils.findFastInPairList
            [
                typeof<float32>,   F32
                typeof<float>,     F64
            ] 
    
type DateType = 
    | DT | DT2
    member __.Name =
        match __ with
        | DT  -> TypeName "DateTime"
        | DT2 -> TypeName "DateTimeOffset"
    member __.DotNetType =
        match __ with
        | DT -> typeof<DateTime> |> Some
        | DT2 -> typeof<DateTimeOffset> |> Some
    static member TryResolve =
        Utils.findFastInPairList 
            [
                typeof<DateTime>,       DT 
                typeof<DateTimeOffset>, DT2
            ] 
    
type WellKnownType =
    | Ordinal of OrdinalType
    | Relational of RelationalType
    | Date of  DateType
    | String 
    | Uuid
    | TimeSpan
    member __.Name =
        match __ with
        | Ordinal ord   -> ord.Name
        | Relational rt   -> rt.Name
        | Date dt       -> dt.Name
        | String        -> TypeName "string"
        | Uuid          -> TypeName "uuid"
        | TimeSpan      -> TypeName "TimeSpan"
    member __.DotNetType =
        match __ with
        | Ordinal ord   -> ord.DotNetType
        | Relational rl   -> rl.DotNetType
        | Date dt       -> dt.DotNetType
        | String        -> typeof<string> |> Some
        | Uuid          -> typeof<Guid> |> Some
        | TimeSpan      -> typeof<TimeSpan> |> Some
    static member private TryResolveInt  =
        Utils.findFastInPairList
            [
                typeof<string>,    String
                typeof<Guid>,      Uuid
                typeof<TimeSpan>,  TimeSpan
            ]
    static member TryResolve  =
        WellKnownType.TryResolveInt   
            =?> (OrdinalType.TryResolve ->> Ordinal)
            =?> (RelationalType.TryResolve ->> Relational)
            =?> (DateType.TryResolve ->> Date)
            
 *)

type DataItemConversionError =
    | InvalidDataItem of string

    
type SDataItem<'t> =
    abstract ToDataItem : 't -> DataItem
    abstract FromDataItem : DataItem -> Result<'t, DataItemConversionError> 
    
module SDataItem =
    let toDataItem<'t, 's when 's :> SDataItem<'t> and 's : struct> o =
        Unchecked.defaultof<'s>.ToDataItem(o)
    let fromDataItem<'t, 's when 's :> SDataItem<'t> and 's : struct> di =
        Unchecked.defaultof<'s>.FromDataItem(di)  

[<AbstractClass>]
type WellKnownType internal (name : string) =
    member __.Name = name
    abstract DotNetType : Type
    
    
type WellKnownType<'t, 's when 's :> SDataItem<'t> and 's : struct> (name) = 
    inherit WellKnownType(name)            
    override __.DotNetType = typeof<'t>
    
type WellKnownTypeDictionary =
    internal {
        fromType : Type -> Option<WellKnownType>
    } 
    
    
         
    
        
    
type ArrayType(lazyElement : Func<DataType>) = 
    let lazyValue = Lazy<_>(lazyElement)
    member __.ElementType  = lazyValue.Value 
    member __.Name = __.ElementType.Name + "[]" 
    member __.Contract  = __.ElementType.Contract  |> Option.map  ( (+) "[]")
        

type OptionType(lazyElement : Func<DataType>) = 
    let lazyValue = Lazy<_>(lazyElement)
    member __.ElementType  = lazyValue.Value 
    member __.Name = __.ElementType.Name + "?" 
    member __.Contract  = __.ElementType.Contract |> Option.map  ( (+) "?")
    


[<CustomEquality>]
[<CustomComparison>]
type NameAndIndex = 
    {
        Name : string
        Index : int option
    }
    override __.Equals(other) =
        match other with
        | :? NameAndIndex as ni ->
            let idxCmp =
                match __.Index with
                | None -> false
                | Some idx ->
                    match ni.Index with
                    | None -> false
                    | Some idx2 -> idx = idx2 
            (__.Name = ni.Name) || idxCmp 
        | _ -> false
    override __.GetHashCode () = __.Name.GetHashCode()
    interface IComparable with
        member __.CompareTo other =
            match other with
            | :? NameAndIndex as ni ->
                if __.Equals(ni) 
                    then 0
                    else
                        compare (__.Name) (ni.Name)
            | _ -> invalidArg "other" "cannot compare values of different types"
        

type MapType =
    {
        Name        : string
        Contract    : string option
        CodeHint    : string option
        IsStruct    : bool
        DotNetType  : Type option
        Fields      : Map<NameAndIndex, Lazy<DataType>>
        RemapFields : Map<string,string> 
    }     
    
type EnumType =
    {
        Name        : string
        Contract    : string option
        CodeHint    : string option
        DotNetType  : Type option
        BasedOn     : Lazy<DataType>
        IsFlags     : bool
        Values      : Map<string, OrdinalItem>
        RemapValues : Map<string, string>
    }
    
type OneOfType =
    {
        Name        : string
        Contract    : string option
        DotNetType  : Type option
        CodeHint    : string option
        Variants    : Map<NameAndIndex, Lazy<DataType>>
        RemapValues : Map<string, string>
    }
    

   
type DataType =     
    | WellKnown of WellKnownType
    | Array of ArrayType
    | Option of OptionType
    | Enum of EnumType
    | Map of MapType
    | OneOf of OneOfType
    member __.Name =
        match __ with
        | WellKnown wk  -> wk.Name
        | Array ar      -> ar.Name
        | Option opt    -> opt.Name
        | Enum en       -> en.Name
        | Map map       -> map.Name
        | OneOf oo      -> oo .Name
        
    member __.Contract =
        match __ with
        | WellKnown wk  -> wk.Name |> Some
        | Array ar      -> ar.Contract 
        | Option opt    -> opt.Contract
        | Enum en       -> en.Contract
        | Map map       -> map.Contract
        | OneOf oo      -> oo.Contract
    member __.CodeHint =
        match __ with
        | WellKnown wk  -> None
        | Array ar      -> None
        | Option opt    -> None
        | Enum en       -> en.CodeHint
        | Map map       -> map.CodeHint
        | OneOf oo      -> oo.CodeHint
    member __.DotNetType =
        match __ with
        | WellKnown wk  -> wk.DotNetType |> Some
        | Array ar      -> None
        | Option opt    -> None
        | Enum en       -> en.DotNetType    
        | Map map       -> map.DotNetType
        | OneOf oo      -> oo.DotNetType

type EventType =
    {
        CodeHint : string option
        EventType : Lazy<DataType>
        ContentType : ContentType option
        Options : Map<string, DataItem>
    }
type CallType =
    {
        CodeHint : string option
        RequestType : Lazy<DataType>
        ResponseType : Lazy<DataType> option
        ContentType : ContentType option
        Options : Map<string, DataItem>
    }

type EndpointType =
    | Event of EventType
    | Call of CallType
    

type ServiceType = 
    {
        Name : string
        Owner : string
        CodeHint : string option
        ContentType : ContentType option
        Endpoints : Map<string, EndpointType> 
        Types : DataType list     
        Options : Map<string, DataItem>            
    }
    
type OrdinalItem =
    | U8 of byte
    | I8 of sbyte
    | U16 of uint16
    | I16 of int16
    | U32 of uint32
    | I32 of int
    | U64 of uint64
    | I64 of int64

type RationalItem =
    | F32 of float32
    | F64 of double

type DateItem =
    | DateTime of DateTime
    | DateTime2 of DateTimeOffset
    
type WellKnownItem =
    | Ordinal of OrdinalItem
    | Rational of RationalItem
    | Date of DateItem
    | String of string
    | Uuid of Guid
    | TimeSpan of TimeSpan

type ArrayItem =
    | Array of DataItem list

type OptionalItem =
    | Optional of DataItem option
    
type ComplexItem =
    | Complex of Map<string, DataItem>

type EnumItem =
    | Enum of OrdinalItem
    
 
    
type OneOfItem =
    {
        TypeHint: string
        Value : DataItem
    }

type DataItem =
    | WellKnown of WellKnownItem
    | Array of ArrayItem
    | Optional of OptionalItem 
    | Complex of ComplexItem  
    | Enum of EnumItem 
    | OneOf of OneOfItem
     

            

    
  