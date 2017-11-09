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
    
    let pid = pidentifier 
    let pdot<'t> : Parser<_, 't> = pchar '.' 
    let pcolon<'t> : Parser<_, 't> = pchar ':'
    let peq<'t> : Parser<_, 't> = pchar '='  
    let pcomma<'t> : Parser<_, 't> = pchar ','
    let psemicolon<'t> : Parser<_, 't> = pchar ';'    
    
    let pqid<'t> : Parser<QualifiedIdentifier, 't> =
        let rec toQId lst =
            match lst with
            | [] -> invalidOp "Empty string mistake"
            | [p] -> QualifiedIdentifier.Simple p
            | h :: t -> QualifiedIdentifier.Complex (toQId t, h)   
        sepBy1 pid pdot |>> (List.rev >> toQId) 
        
        
        
    let pOrdinalType<'t> : Parser<OrdinalType, 't> =
        choice 
            [
                pstring "u8" >>% OrdinalType.U8
                pstring "i8" >>% OrdinalType.I8
                pstring "u16" >>% OrdinalType.U16
                pstring "i16" >>% OrdinalType.I16
                pstring "u32" >>% OrdinalType.U32
                pstring "i32" >>% OrdinalType.I32
                pstring "u64" >>% OrdinalType.U64
                pstring "i64" >>% OrdinalType.I64
            ]
    let pBasicType<'t> : Parser<BasicType, 't> =
        choice
            [
                pOrdinalType |>> (fun p -> BasicType.Ordinal p)
                pstring "f32" >>% F32
                pstring "f64" >>% F64
                pstring "string" >>% String
                pstring "uuid" >>% Uuid
                pstring "datetime" >>% DT
                pstring "utcdatetime" >>% DTO
                pstring "timespan" >>% TS
                pstring "bool" >>% Bool
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
                attempt (numberLiteral fmt "ordinal" .>>. pOrdinalType |> replyError cvtPrefix)      
                attempt (numberLiteral fmt "ordinal" |> replyError (fun p -> try_ (fun () -> int32 p.String |> I32Literal)) .>> notFollowedBy pdot)          
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
        attempt (choice 
            [
                pstringCI "true" >>% BoolLiteral true
                pstringCI "false" >>% BoolLiteral false
                pstringCI "yes" >>% BoolLiteral true
                pstringCI "no" >>% BoolLiteral true
            ])
            
    let pNoneLiteral<'t> : Parser<_, 't> =
           attempt (pstring "none" >>% NoneLiteral)
            
    let pBasicLiteral<'t> : Parser<BasicLiteral, 't> =
        choice 
            [
                pStringLiteral |>> StringLiteral
                pOrdinalLiteral |>> OrdinalLiteral
                pFloatLiteral
                pNoneLiteral 
                pBoolLiteral
            ]     
          
    
    let inBracers os cs p =
        between (pchar os .>> spaces) (pchar cs) (p .>> spaces)  
        
    let pLiteral<'t> : Parser<_, 't> =
        let (literal, literalRef) = createParserForwardedToRef<Literal, 't>()
        let arrayLiteral =
            inBracers '[' ']' ((sepBy (spaces >>. literal .>> spaces) pcomma)) |>> ArrayLiteral 
        let field =
                 pid .>> spaces .>> peq .>> spaces .>>. literal .>> spaces 
        let mapLiteral =
            inBracers '{' '}' (sepBy (spaces >>. field .>> spaces) pcomma) |>> MapLiteral
        
        literalRef := 
            choice
                [
                    pBasicLiteral |>> BasicLiteral
                    pqid |>> IdentifierLiteral
                    arrayLiteral
                    mapLiteral
                ]
        literal
        
        
    let pMayBeType<'t> tParser : Parser<TypeSpecification, 't>  =
        pchar '?' >>. tParser .>> notFollowedBy (pchar '?') |>> MayBeType 
    
    let pArrayType<'t> tParser : Parser<TypeSpecification, 't> =
        pstring "[]" >>. spaces >>. tParser   |>> ArrayType
        
    let pEnumType<'t> :Parser<TypeSpecification, 't> =
        let field =
            pidentifier .>>. (spaces >>. pstring "=" >>. spaces >>. pOrdinalLiteral) 
            
        let pEnumOpt =
            pstring "(" .>> spaces >>. choice 
                [
                    attempt (pBoolLiteral .>> spaces .>> pstring ")" |>> (fun b -> match b with | BoolLiteral b1 ->  (b1, I32) | _ -> (false, I32)))
                    attempt (pBoolLiteral .>> spaces .>> pstring "," .>> spaces .>>. pOrdinalType .>> spaces .>> pstring ")" |>> (fun (b, t) -> match b with | BoolLiteral b1 ->  (b1, t) | _ -> (false, t)))
                    attempt (pOrdinalType .>> spaces .>> pstring "," .>> spaces .>>. pBoolLiteral .>> spaces .>> pstring ")" |>> (fun (t, b) -> match b with | BoolLiteral b1 ->  (b1, t) | _ -> (false, t)))
                    attempt (pOrdinalType .>> spaces .>> pstring ")" |>> (fun t -> (false, t)))
                ]    
         
            
        pstring "enum" .>> spaces  >>. (opt  pEnumOpt) .>> spaces .>>.  inBracers '{' '}' (spaces >>.(sepEndBy1 field (psemicolon .>> spaces)) .>> spaces)  
            |>> (fun (f, filds) ->
                    match f with
                    | Some (b, t) -> EnumType(b, t, filds)
                    | None -> EnumType(false, I32, filds))
    
    let pOneOfType<'t> tParser : Parser<TypeSpecification, 't> =
       let field = (pidentifier .>>. (spaces >>. pstring ":" >>. spaces >>. tParser)) 
       let start = (pstring "oneOf" <|> pstring "oneof") 
       let unitEl = ((pidentifier .>> notFollowedBy (spaces .>> pstring ":")) |>> (fun p -> p, UnitType)) 
       let fld = ((attempt unitEl) <|> (attempt field)) 
       let inner = spaces >>. (sepBy fld (pstring ";" .>> spaces)) .>> spaces
                   
       start .>> spaces1 >>. between (pstring "{") (pstring "}") inner |>> OneOfType
           
    let pMapType<'t> tParser : Parser<TypeSpecification, 't> =
           let field =
                       pidentifier .>>. (spaces >>. pstring ":" >>. spaces >>. tParser) 
           pstring "map" .>> spaces1 >>. between (pstring "{") (pstring "}") (spaces >>.(sepEndBy1 field (pstring ";" .>> spaces)) .>> spaces) 
               |>> MapType       
            
    let pTypeSpecification<'t> : Parser<_, 't> =
        let full, fullOther = createParserForwardedToRef<TypeSpecification, 't>()
        let op =
            choice 
                [
                    
                    pOneOfType full   |> attempt
                    pMapType full  |> attempt
                    pEnumType   |> attempt
                    pMayBeType full |> attempt
                    pArrayType full
                    pstring "unit" >>% TypeSpecification.UnitType 
                    pBasicType |>> BasicType |> attempt 
                    (pqid |>> TypeName|> attempt)  
                    
                    
                    
                ]
        fullOther := op
        full   
        
    let pOpenDirective<'t> : Parser<_, 't> =
        pstring "open" .>> spaces1 >>. pqid .>> spaces .>> psemicolon |>> OpenDirective
        
    let pUseDerective<'t> : Parser<_, 't> =
        pstring "use" .>> spaces1 >>. pqid .>> spaces .>> psemicolon |>> UseDirective
        
    let pExtensionParser<'t> : Parser<QualifiedIdentifier * Literal, 't> =
        pqid .>> spaces .>> peq .>> spaces .>>. pLiteral
        
    let pExtensionsParser<'t> : Parser<_, 't> =
        sepBy1 pExtensionParser (spaces1)
        
    let pEventHeaderParser<'t> : Parser<_, 't> =
        pstring "event" .>> spaces1 >>. pidentifier .>> spaces .>> pcolon .>> spaces .>>. pTypeSpecification
        
    let pEventParser<'t> : Parser<_, 't> =
        pEventHeaderParser .>> spaces1 .>>. pExtensionsParser .>> spaces .>> pstring ";" 
            |>> (fun ((i, t), ex) -> EventDeclaration (i, { EventType = t; Extensions = ex }))   
            
    let pTypeParser<'t> : Parser<_, 't> =
        pstring "type" .>> spaces1 >>. pidentifier .>>spaces .>> peq .>> spaces .>>. pTypeSpecification 
            |>> (fun (i, t) -> NamedTypeDeclaration (i, t))  
     
     
    let pArgumentListParser<'t> : Parser<_, 't> =
        let simpleArg = inBracers '(' ')'  pTypeSpecification |>> (fun p -> [None, p])
        let idName : Parser<_, 't> = pidentifier |>> Some .>> spaces .>> pcolon
        let tSpec : Parser<_, 't> = spaces >>.pTypeSpecification .>> spaces
        let pairSpec = idName .>>. tSpec  
        let pairList = sepBy1 pairSpec pcomma 
          
        let argList = inBracers '(' ')' pairList
        choice
            [
                attempt simpleArg
                attempt argList
            ]
            
    let pCallParser<'t> : Parser<_, 't> =
        pstring "call" .>> spaces1 >>. pTypeSpecification .>> spaces1 .>>. pidentifier .>> spaces .>>. pArgumentListParser 
            .>> spaces1 .>>. pExtensionsParser .>> spaces .>> psemicolon
            |>> fun g -> let (((rt, i), args), ex) = g in  CallDeclaration (i, { Parameters = args; Result = rt; Extensions = ex })
            
    let pServiceDeclaration<'t> : Parser<_, 't> =
        let pep = (choice [pCallParser; pEventParser]) .>> spaces1
        let pepl = many pep  
        pstring "service" >>. spaces >>. pidentifier .>> spaces1 .>>. (opt pExtensionsParser |>> (Option.defaultValue [])) .>> spaces
            .>>. (inBracers '{' '}' pepl) 
            |>> fun ((i, ext), ep) ->  NamespaceElement.ServiceDeclaration (i, ServiceDeclaration.ServiceDeclaration (ep, ext))
            
    let pNamespace<'t> : Parser<_, 't> =
        pstring "namespace" .>> spaces1 >>. pqid .>> spaces .>>. 
            inBracers '{' '}' (many (choice [pTypeParser; pServiceDeclaration; pOpenDirective |>> NamespaceElement.Open ] .>> spaces))
            |>> fun  (i, el) -> NamespaceDeclaration (i, el)
            
    let pCodeUnit<'t> : Parser<_, 't> =
            many (choice
                [
                    pUseDerective .>> spaces
                    pOpenDirective .>> spaces |>> Open  
                    pNamespace .>> spaces
                ]) .>> spaces .>> eof |>> CodeUnit 
            