namespace Astral.Schema.Ast
open System





type OrdinalValueNode =
    | U8Value of byte
    | I8Value of sbyte
    | U16Value of byte
    | I16Value of byte
    | U32Value of byte
    | I32Value of byte
    | U64Value of byte
    | I64Value of byte

type SimpleValueNode =
    | OrdinalValue of OrdinalValueNode
    | F32Value of float32
    | F64Value of float
    | StringValue of string
    | UuidValue of Guid
    | DTValue of DateTime
    | DTOValue of DateTimeOffset
    | TSValue of TimeSpan
and ArrayValueNode =
    | ArrayValue of ValueNode list
and MapValueNode =
    | MapValue of Map<string, ValueNode>     
and ValueNode =
    | Simple of SimpleValueNode   
    | Array of ArrayValueNode
    | Map of MapValueNode 

type Identifier = Identifier of string
type NamespaceName = Identifier list 
type QualifiedIdentifier = NamespaceName * Identifier

type OrdinalTypeNode =
        | U8 | I8 | U16 | I16 | U32 | I32 | U64 | I64
        member __.Name =
            match __ with
            | U8  -> "u8" | I8  -> "i8"  | U16 -> "u16" | I16 -> "i16" 
            | U32 -> "u32"| I32 -> "i32" | U64 -> "u64" | I64 -> "i64"
        member __.Type =
            match __ with
            | U8  -> typeof<byte>   | I8  -> typeof<sbyte>  | U16 -> typeof<uint16> | I16 -> typeof<int16> 
            | U32 -> typeof<uint32> | I32 -> typeof<int32>  | U64 -> typeof<uint64> | I64 -> typeof<int64>
            
type SimpleTypeNode =
    | Ordinal of OrdinalTypeNode
    | F32 | F64 | String | Uuid | DT | DTO | TS
    member __.Name =
      match __ with
      | Ordinal ord -> ord.Name | F32 -> "f32"      | F64 -> "f64"       | String -> "string"
      | Uuid        -> "uuid"   | DT  -> "datetime" | DTO -> "datetime2" | TS     -> "timespan"  
and TypeOrTypeName =
    | Type of TypeNodeGreen
    | Name of QualifiedIdentifier 
and ArrayTypeNodeGreen = Array of TypeOrTypeName
and MayBeTypeNodeGreen = Maybe of TypeOrTypeName
and EnumTypeField = Identifier * OrdinalValueNode
and EnumTypeNodeGreen =  
    {
        Name : QualifiedIdentifier option
        CodeHint : QualifiedIdentifier option
        Type : Type option
        BaseType : OrdinalTypeNode
        IsFlags : bool
        Values : EnumTypeField list        
    } 
and NamedTypeField = Identifier * TypeOrTypeName
and MapTypeField = NamedTypeField * Option<int>
and MapTypeNodeGreen =
    {
        Name : QualifiedIdentifier option
        CodeHint : QualifiedIdentifier option
        Type : Type option
        IsStruct : bool
        Fields : MapTypeField list
    }
and OneOfTypeNodeGreen =
    {
        Name : QualifiedIdentifier option
        CodeHint : QualifiedIdentifier option
        Type : Type option
        Variants : NamedTypeField list
    }
and DeclareTypeNodeGreen =
    | Map of MapTypeNodeGreen
    | Enum of EnumTypeNodeGreen
    | OneOf of OneOfTypeNodeGreen
and TypeNodeGreen = 
    | Simple of SimpleTypeNode 
    | Array of ArrayTypeNodeGreen
    | MayBe of MayBeTypeNodeGreen
    | Enum of EnumTypeNodeGreen
    | Map of MapTypeNodeGreen
    | OneOf of OneOfTypeNodeGreen
    

type ExtensionNode = QualifiedIdentifier * ValueNode
type ContentType = ContentType of string

type EventNode =
    {
        Name : Identifier
        ContentType : ContentType option
        CodeHint : Identifier option
        EventType : TypeOrTypeName
        Extensions : ExtensionNode list
    }
and CallNode =
    {
        Name : Identifier
        ContentType : ContentType option
        CodeHint : Identifier option
        RequestType : TypeOrTypeName
        ResponseType : TypeOrTypeName option
        Extensions : ExtensionNode list
    }
and EndpointNode =
    | Event of EventNode
    | Call of CallNode
and ServiceNode =
    {
        Name : Identifier
        ContentType : ContentType option
        CodeHint : Identifier option
        Extensions : ExtensionNode list
        Endpoints : EndpointNode list
    }
and NamespaceElement =
    | Service of ServiceNode
    | DeclareType of DeclareTypeNodeGreen

type Namespace = QualifiedIdentifier * List<NamespaceElement>

type Schema = Namespace list