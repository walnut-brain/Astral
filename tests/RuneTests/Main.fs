module RuneTests
open Expecto
open FParsec
open Astral.Schema.Ast.Ast
open Astral.Schema.Ast.Parsing

let runParser p str =
    match run p str with
    | Success(result, _, _)   -> printfn "Success: %A" result
    | Failure(errorMsg, _, _) -> printfn "Failure: %s" errorMsg  

[<EntryPoint>]
let main argv =
    runParser (spaces >>. pEventParser) """
        event Formal : [] oneof 
            {  
                Ok; 
                frm : u64; 
                ffr : ?enum 
                        { 
                            one = 1; 
                            two = 2u64 
                        } 
            } 
        exchange="123" routingKey=12345;""" 
    //runParser pEventParser """call Formal: [] oneof { frm : u64; ffr : ?enum { one = 1; two = 2u64 } } exchange="123" routingKey=12345;"""
    //runParser pEventParser """call oneof { Ok ; Formal: [] oneof { frm : u64; ffr : ?enum { one = 1; two = 2u64 } } exchange="123" routingKey=12345;""" 
    Tests.runTestsInAssembly defaultConfig argv
