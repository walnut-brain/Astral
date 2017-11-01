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
    runParser pLiteral   """[   54.0, 
                                "hellow", 
                                true, 
                                none, 
                                { 
                                    v = [2.2, 7.5], 
                                    b = no 
                                }]"""
    //runParser pBasicLiteral "54"
    Tests.runTestsInAssembly defaultConfig argv
