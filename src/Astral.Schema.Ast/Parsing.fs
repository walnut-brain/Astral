namespace Astral.Schema.Ast

module Parsing =
    open FParsec
    open Ast
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
     
    let pOrdinalLiteral<'t> : Parser<OrdinalLiteral, 't> =
        let fmt =   NumberLiteralOptions.DefaultInteger ||| NumberLiteralOptions.AllowSuffix
        let cvt (nl : NumberLiteral) =
            let checkSigned () =
                match nl.SuffixChar1 with
                | 'i' -> true
                | 'u' -> false
                | _ -> invalidOp (sprintf "Invalid suffix char %A" nl.SuffixChar1)
            match nl.SuffixLength with
            | 0 -> int32 nl.String |> I32Literal
            | 2 ->
                let signed = checkSigned()
                if nl.SuffixChar2 = '8' then
                    if signed then int8 nl.String |> I8Literal else uint8 nl.String |> U8Literal
                else    
                    invalidOp "Invalid depth in suffix"
            | 3 ->
               let signed = checkSigned()
               match nl.SuffixChar2.ToString() + nl.SuffixChar3.ToString() with
               | "16" -> if signed then int16 nl.String |> I16Literal else uint16 nl.String |> U16Literal
               | "32" -> if signed then int32 nl.String |> I32Literal else uint32 nl.String |> U32Literal
               | "64" -> if signed then int64 nl.String |> I64Literal else uint64 nl.String |> U64Literal
               | _ -> invalidOp "Invalid depth in suffix"         
            | _ -> invalidOp "Invalid suffix length" 
        numberLiteral fmt "ordinal" 
        |>> (fun c -> cvt c)
    let pFloatLiteral<'t> : Parser<BasicLiteral, 't> =
            let fmt =   NumberLiteralOptions.DefaultFloat ||| NumberLiteralOptions.AllowSuffix
            let cvt (nl : NumberLiteral) =
                if nl.IsInteger then invalidOp "Invalid float literal"
                match nl.SuffixLength with
                | 0 ->
                    float nl.String |> F64Literal
                | 3 ->
                   match nl.SuffixChar1.ToString() +  nl.SuffixChar2.ToString() + nl.SuffixChar3.ToString() with
                   | "f32" -> float32 nl.String |> F32Literal
                   | "f64" -> float nl.String |> F64Literal
                   | _ -> invalidOp "Invalid float literal"         
                | _ -> invalidOp "Invalid suffix length" 
            numberLiteral fmt "float literal" 
            |>> (fun c -> cvt c)        
    
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
                attempt pFloatLiteral 
                attempt (pOrdinalLiteral |>> OrdinalLiteral)
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