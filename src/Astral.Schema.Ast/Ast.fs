namespace Astral.Schema.Ast

module rec Ast =
    open System
    open FParsec


    type Identifier = private Identifier of string 
    
    let pidentifier<'t> : Parser<Identifier, 't> =
        let isAsciiIdStart    = fun c -> isAsciiLetter c || c = '_'
        let isAsciiIdContinue = fun c -> isAsciiLetter c || isDigit c || c = '_'
        
        identifier (IdentifierOptions(
                            isAsciiIdStart = isAsciiIdStart,
                            isAsciiIdContinue = isAsciiIdContinue
                            )) |>> Identifier  
    
    let createIdentifier s =
        match run pidentifier s with
        | Success (id, _, _) -> id
        | Failure(msg, _, _) -> raise (ArgumentException msg)  
    
    type QualifiedIdentifier =
        | Simple of Identifier
        | Complex of QualifiedIdentifier * Identifier
        
    type OrdinalType =
        | U8 | I8 | U16 | I16 | U32 | I32 | U64 | I64
    
    type BasicType =
        | Ordinal of OrdinalType
        | F32 | F64 | String | Uuid | DT | DTO | TS | Bool
        
    type OrdinalLiteral =
        | U8Literal of byte
        | I8Literal of sbyte
        | U16Literal of uint16
        | I16Literal of int16
        | U32Literal of uint32
        | I32Literal of int32
        | U64Literal of uint64
        | I64Literal of int64
                
    type BasicLiteral =
            | OrdinalLiteral of OrdinalLiteral
            | F32Literal of float32
            | F64Literal of float
            | StringLiteral of string
            | UuidLiteral of Guid
            | BoolLiteral of bool
            | DTLiteral of DateTime
            | DTOLiteral of DateTimeOffset
            | TSLiteral of TimeSpan

    type Literal =
        | NoneLiteral
        | IdentifierLiteral of QualifiedIdentifier
        | BasicLiteral of BasicLiteral 
        | ArrayLiteral of Literal list
        | MapLiteral of List<Identifier * Literal> 


    type OpenDirective = | OpenDirective of QualifiedIdentifier 
    
    type TypeSpecification =
        | UnitType
        | TypeName of QualifiedIdentifier
        | BasicType of BasicType
        | ArrayType of TypeSpecification
        | MayBeType of TypeSpecification
        | EnumType of bool * List<Identifier * OrdinalLiteral>
        | MapType of List<Identifier * TypeSpecification>
        | OneOfType of List<Identifier * TypeSpecification>
    
    
    type EventDeclaration =
        {
            EventType : TypeSpecification
            Extensions : List<QualifiedIdentifier * Literal>
        } 
    
    type CallDeclaration =
        {
            Parameters : List<Identifier * TypeSpecification>
            Result : TypeSpecification
            Extensions : List<QualifiedIdentifier * Literal>
        }
        
        
    type EndpointDeclaration =
        | EventDeclaration of Identifier * EventDeclaration
        | CallDeclaration of Identifier * CallDeclaration
    
    type ServiceDeclaration = 
        ServiceDeclaration of List<EndpointDeclaration> * List<QualifiedIdentifier * Literal>
    
    type NamespaceElement =
        | Open of  OpenDirective
        | ServiceDeclaration of Identifier * ServiceDeclaration
        | NamedTypeDeclaration of Identifier * TypeSpecification
    
    
    type CodeUnitElement =
        | NamespaceDeclaration of QualifiedIdentifier
        | Open of OpenDirective
        | UseDirective of QualifiedIdentifier
        
    type CodeUnit = CodeUnit of CodeUnitElement list
        
    
    
    