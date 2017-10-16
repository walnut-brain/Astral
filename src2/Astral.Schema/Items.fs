namespace Astral.Schema
open System

type PrimeItem =
    | U8 of byte
    | I8 of sbyte
    | U16 of uint16
    | I16 of int16
    | U32 of uint32
    | I32 of int
    | U64 of uint64
    | I64 of int64
    | F32 of float32
    | F64 of double
    | DT of DateTime
    | DTO of DateTimeOffset
    | String of string
    | Uuid of Guid
    | TimeSpan of TimeSpan
    | Nothing 

type ArrayItem =
    | Array of DataItem list
and
    OptionalItem =
    | Optional of DataItem option
and    
    ComplexItem =
    | Complex of Map<string, DataItem>
and    
    OneOfItem =
    {
        TypeHint: string
        Value : DataItem
    }
and
    DataItem =
    | Prime of PrimeItem
    | Array of ArrayItem
    | Optional of OptionalItem 
    | Complex of ComplexItem  
    | OneOf of OneOfItem