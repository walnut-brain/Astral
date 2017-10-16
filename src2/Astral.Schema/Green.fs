namespace Astral.Schema
open System

  
type DataItemConversionError =
    | InvalidDataItem of string
    | UnknownType of Type    

[<CustomEquality>]
[<NoComparison>]
type internal WellKnownGreen = 
    {
            Name : string
            DotNetType : Type
            ToDataItem : obj -> Option<DataItem>
            FromDataItem : DataItem -> Result<obj, DataItemConversionError>
    }
    override __.Equals(other) =
            match other with
            | :? WellKnownGreen as wk -> wk.DotNetType = __.DotNetType  
            | _ -> false
    override __.GetHashCode () = __.DotNetType.GetHashCode()
and internal ArrayGreen =
        | ArrayOf of string
        member __.Name = let (ArrayOf name) = __ in name + "[]" 
and internal OptionGreen =
        | OptionOf of string
        member __.Name = let (OptionOf name) = __ in name + "?"
and internal EnumGreen =
        {
            Name : string
            Values : Map<string, int64>                    
        }
and internal FieldGreen =
        {
            Index : Option<uint32>
            Type : string                
        }
and internal MapGreen =
        {
            Name : string
            Fields : Map<string, FieldGreen>    
        }
and internal OneOfGreen =
        {
            Name : string
            Variants : Map<string, string>    
        }        
and internal TypeGreen =
        | WellKnownGreen 
        | ArrayGreen
        | OptionGreen
        | EnumGreen
        | MapGreen
        | OneOfGreen
        