namespace Astral.Schema.Ast

module Parsing =
    open System
    open FParsec
    open Ast
    let private (<!>) (p: Parser<_,_>) label : Parser<_,_> =
        fun stream ->
            printfn "%A: Entering %s" stream.Position label
            let reply = p stream
            printfn "%A: Leaving %s (%A)" stream.Position label reply.Status
            reply
    let pQualifiedIdentifier<'t> : Parser<QualifiedIdentifier, 't> =
        let rec toQId lst =
            match lst with
            | [] -> invalidOp "Empty string mistake"
            | [p] -> QualifiedIdentifier.Simple p
            | h :: t -> QualifiedIdentifier.Complex (toQId t, h)   
        sepBy1 pidentifier (pchar '.') |>> (List.rev >> toQId) 
    let pOrdinalType<'t> : Parser<OrdinalType, 't> =
        choice 
            [
                pstring "u8" |>> (fun _ -> OrdinalType.U8)
                pstring "i8" |>> (fun _ -> OrdinalType.I8)
                pstring "u16" |>> (fun _ -> OrdinalType.U16)
                pstring "i16" |>> (fun _ -> OrdinalType.I16)
                pstring "u32" |>> (fun _ -> OrdinalType.U32)
                pstring "i32" |>> (fun _ -> OrdinalType.I32)
                pstring "u64" |>> (fun _ -> OrdinalType.U64)
                pstring "i64" |>> (fun _ -> OrdinalType.I64)
            ]
    let pBasicType<'t> : Parser<BasicType, 't> =
        choice
            [
                pOrdinalType |>> (fun p -> BasicType.Ordinal p)
                pstring "f32" |>> (fun _ -> F32)
                pstring "f64" |>> (fun _ -> F64)
                pstring "string" |>> (fun _ -> String)
                pstring "uuid" |>> (fun _ -> Uuid)
                pstring "datetime" |>> (fun _ -> DT)
                pstring "utcdatetime" |>> (fun _ -> DTO)
                pstring "timespan" |>> (fun _ -> TS)
                pstring "bool" |>> (fun _ -> Bool)
            ]
            
    let replyError<'s, 'a, 'b>   (cvt: 'a -> Result<'b, string>) (p : Parser<'a, 's>) : Parser<'b, 's> =
        fun stream ->
            let r1 = p stream
            if r1.Status = Ok then
                match cvt (r1.Result) with
                | Result.Ok ok -> Reply(ok)
                | Result.Error e -> Reply(ReplyStatus.Error, mergeErrors r1.Error (expected e))
            else
                Reply(r1.Status, r1.Error)
                
                
    let try_ f  =
        try
           f() |> Result.Ok
        with
        | ex -> ex.Message |> Result.Error 
            
    let pOrdinalLiteral<'t> : Parser<OrdinalLiteral, 't> =
        let fmt =   NumberLiteralOptions.DefaultInteger
        let cvtPrefix (nl : NumberLiteral, o) =
            match o with
            | U8 -> try_ (fun () -> uint8 nl.String |> U8Literal)
            | I8 -> try_ (fun () -> int8 nl.String |> I8Literal)
            | U16 -> try_ (fun () -> uint16 nl.String |> U16Literal)
            | I16 -> try_ (fun () -> int16 nl.String |> I16Literal)
            | U32 -> try_ (fun () -> uint32 nl.String |> U32Literal)
            | I32 -> try_ (fun () -> int32 nl.String |> I32Literal)
            | U64 -> try_ (fun () -> uint64 nl.String |> U64Literal)
            | I64 -> try_ (fun () -> int64 nl.String |> I64Literal)
        choice
            [
                attempt (numberLiteral fmt "ordinal" .>>. pOrdinalType |> replyError cvtPrefix .>> notFollowedBy (pstring "."))     
                attempt (numberLiteral fmt "ordinal" |> replyError (fun p -> try_ (fun () -> int32 p.String |> I32Literal)) .>> notFollowedBy (pstring "."))          
            ] 
          
    let pFloatLiteral<'t> : Parser<BasicLiteral, 't> =
            let fmt =   NumberLiteralOptions.DefaultFloat
            let cvtPrefix (nl : NumberLiteral, s) =
                match s with
                | "f32" -> try_ (fun () -> float32 nl.String |> F32Literal)
                | "f64" -> try_ (fun () -> float nl.String |> F64Literal)
                | s1 -> Result.Error (sprintf "Invalid suffix %A" s1) 
             
            choice
                [
                    attempt ((numberLiteral fmt "float" .>>. (pstring "f32" <|> pstring "f64")) |> replyError cvtPrefix)     
                    attempt (numberLiteral fmt "float" |> replyError (fun p -> try_ (fun () -> float p.String |> F64Literal)))          
                ]        
    
    let pStringLiteral<'t> : Parser<string, 't> =
        let escape =  anyOf "\"\\/bfnrt"
                      |>> function
                          | 'b' -> "\b"
                          | 'f' -> "\u000C"
                          | 'n' -> "\n"
                          | 'r' -> "\r"
                          | 't' -> "\t"
                          | c   -> string c // every other char is mapped to itself
        
        let unicodeEscape =
            /// converts a hex char ([0-9a-fA-F]) to its integer number (0-15)
            let hex2int c = (int c &&& 15) + (int c >>> 6)*9
    
            pstring "u" >>. pipe4 hex hex hex hex (fun h3 h2 h1 h0 ->
                (hex2int h3)*4096 + (hex2int h2)*256 + (hex2int h1)*16 + hex2int h0
                |> char |> string
            )
        
        let escapedCharSnippet = pstring "\\" >>. (escape <|> unicodeEscape)
        let normalCharSnippet  = manySatisfy (fun c -> c <> '"' && c <> '\\')
        
        between (pstring "\"") (pstring "\"")
                (stringsSepBy normalCharSnippet escapedCharSnippet)

    let pBoolLiteral<'t> : Parser<_, 't> =
        choice 
            [
                pstringCI "true" |>> (fun _ -> BoolLiteral true)
                pstringCI "false" |>> (fun _ -> BoolLiteral false)
                pstringCI "yes" |>> (fun _ -> BoolLiteral true)
                pstringCI "no" |>> (fun _ -> BoolLiteral true)
            ]
    let pBasicLiteral<'t> : Parser<BasicLiteral, 't> =
        choice 
            [
                pStringLiteral |>> StringLiteral
                pOrdinalLiteral |>> OrdinalLiteral
                pFloatLiteral 
                pBoolLiteral
            ]     
    let pNoneLiteral<'t> : Parser<_, 't> =
        pstring "none" |>> (fun _ -> NoneLiteral)      
        
    let pLiteral<'t> : Parser<_, 't> =
        let (literal, literalRef) = createParserForwardedToRef<Literal, 't>()
        let arrayLiteral1 = 
            between (pstring "[") (pstring "]")
                (spaces >>. sepBy (literal .>> spaces) (pstring "," >>. spaces))
        let arrayLiteral = arrayLiteral1 |>> ArrayLiteral
        let field =
                 pidentifier .>>. (spaces >>. pstring "=" >>. spaces >>. literal) 
        let mapLiteral1 =
            between (pstring "{") (pstring "}")
                (spaces >>. sepBy (field .>> spaces) (pstring "," >>. spaces))
        let mapLiteral = mapLiteral1 |>> MapLiteral
        literalRef := 
            choice
                [
                    pNoneLiteral 
                    pBasicLiteral |>> BasicLiteral
                    pQualifiedIdentifier |>> IdentifierLiteral
                    arrayLiteral
                    mapLiteral
                ]
        literal
        
        
    let pMayBeType<'t> tParser : Parser<TypeSpecification, 't>  =
        pstring "?" >>. tParser .>> notFollowedBy (pstring "?") |>> MayBeType 
    
    let pArrayType<'t> tParser : Parser<TypeSpecification, 't> =
        pstring "[]" >>. spaces >>. tParser   |>> ArrayType
        
    let pEnumType<'t> :Parser<TypeSpecification, 't> =
        let field =
            pidentifier .>>. (spaces >>. pstring "=" >>. spaces >>. pOrdinalLiteral) 
            
        pstring "enum" .>> spaces1 >>. (opt  (pstring "flags" .>> spaces1)) .>>. between (pstring "{") (pstring "}") (spaces >>.(sepBy1 field (pstring ";" .>> spaces)) .>> spaces)  
            |>> (fun (f, filds) ->
                    match f with
                    | Some _ -> EnumType(true, filds)
                    | None -> EnumType(false, filds))
    
    let pOneOfType<'t> tParser : Parser<TypeSpecification, 't> =
       let field = (pidentifier .>>. (spaces >>. pstring ":" >>. spaces >>. tParser)) <!> "fld"
       let start = (pstring "oneOf" <|> pstring "oneof") <!> "st"
       let unitEl = ((pidentifier .>> notFollowedBy (spaces .>> pstring ":")) |>> (fun p -> p, UnitType)) <!> "unit"  
       let fld = ((attempt unitEl) <|> (attempt field)) <!> "pair"
       let inner = spaces >>. (sepBy fld (pstring ";" .>> spaces)) .>> spaces
                   
       start .>> spaces1 >>. between (pstring "{") (pstring "}") inner |>> OneOfType
           
    let pMapType<'t> tParser : Parser<TypeSpecification, 't> =
           let field =
                       pidentifier .>>. (spaces >>. pstring ":" >>. spaces >>. tParser) 
           pstring "map" .>> spaces1 >>. between (pstring "{") (pstring "}") (spaces >>.(sepBy1 field (pstring ";" .>> spaces)) .>> spaces) 
               |>> OneOfType       
            
    let pTypeSpecification<'t> : Parser<_, 't> =
        let full, fullOther = createParserForwardedToRef<TypeSpecification, 't>()
        let op =
            choice 
                [
                    pOneOfType full   |> attempt <!> "OneOf"
                    pMapType full  |> attempt <!> "Map"
                    pEnumType   |> attempt <!> "Enum"
                    pMayBeType full |> attempt <!> "MayBe"
                    pArrayType full <!> "Array"
                    pBasicType |>> BasicType |> attempt <!> "Basic"
                    (pQualifiedIdentifier |>> TypeName|> attempt) <!> "Qualified" 
                    
                    
                    
                ]
        fullOther := op
        full   
        
    let pOpenDirective<'t> : Parser<_, 't> =
        pstring "open" .>> spaces1 >>. pQualifiedIdentifier |>> OpenDirective
        
    let pUseDerective<'t> : Parser<_, 't> =
        pstring "use" .>> spaces1 >>. pQualifiedIdentifier |>> UseDirective
        
    let pExtensionParser<'t> : Parser<QualifiedIdentifier * Literal, 't> =
        pQualifiedIdentifier .>> spaces .>> pstring "=" .>> spaces .>>. pLiteral
        
    let pExtensionsParser<'t> : Parser<_, 't> =
        sepBy1 pExtensionParser (spaces1)
        
    let pEventHeaderParser<'t> : Parser<_, 't> =
        pstring "event" .>> spaces1 >>. pidentifier .>> spaces .>> pstring ":" .>> spaces .>>. pTypeSpecification
        
    let pEventParser<'t> : Parser<_, 't> =
        pEventHeaderParser .>> spaces1 .>>. pExtensionsParser .>> spaces .>> pstring ";" 
            |>> (fun ((i, t), ex) -> EventDeclaration (i, { EventType = t; Extensions = ex }))     
     