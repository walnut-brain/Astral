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
      



type WellKnownType internal (green: WellKnownGreen)  = 
    member __.Name = green.Name
    member __.DotNetType = green.DotNetType 
    
type ArrayType internal (root : ServiceType, green : ArrayGreen) = 
    //member __.ElementType  =   
    member __.Name = green.Name 
    //member __.Contract  = __.ElementType.Contract  |> Option.map  ( (+) "[]")
        

type OptionType internal (root : ServiceType, green : ArrayGreen) = 
    //member __.ElementType  = lazyValue.Value 
    member __.Name = green.Name 
    //member __.Contract  = __.ElementType.Contract |> Option.map  ( (+) "?")
    
     

type MapType internal (root : ServiceType, green : MapGreen) =
     member __.Name = green.Name    
    
type EnumType internal (root : ServiceType, green : EnumGreen) =
     member __.Name = green.Name
    
type OneOfType internal (root : ServiceType, green : OneOfGreen) =
     member __.Name = green.Name
    
    

   
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
    

     

            

    
  