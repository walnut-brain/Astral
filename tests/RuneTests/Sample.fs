module Tests

open Expecto
open FParsec
open Astral.Schema.Ast.Ast
open Astral.Schema.Ast.Parsing

let check name parser text awaited =
  testCase  name <| fun _ ->
      match run parser text with
      | Success(result, _, _)   -> Expect.equal (Result.Ok result) awaited  "success"
      | Failure(errorMsg, _, _) -> Expect.equal (Result.Error errorMsg) awaited "success"
let checkSuccess name parser text awaited =
   testCase  name <| fun _ ->
       match run parser text with
       | Success(result, _, _)   -> Expect.equal result awaited "not equal"
       | Failure(errorMsg, _, _) -> Tests.failtest errorMsg
let checkFail name parser text =
    testCase name <| fun _ ->
      match run parser text with
      | Success(result, _, _)   -> Tests.failtest "must error"
      | Failure(errorMsg, _, _) -> Expect.equal true true "ok"
       
         

[<Tests>]
let tests =
  
  testList "parsers" [
    checkSuccess "identifier parsed" pidentifier "Abcd" (createIdentifier "Abcd")
    checkFail "identifier bad" pidentifier "5Abcd"
    checkSuccess "qualified identifier" pQualifiedIdentifier "Fgh.Abcd" (Complex (Simple (createIdentifier "Fgh"), (createIdentifier "Abcd")))
    checkFail "qualified identifier" pQualifiedIdentifier "Fgh.,Abcd"
    (*testCase "identifier parsed" <| fun _ ->
      let subject = true
      Expect.isTrue subject "I compute, therefore I am."

    testCase "when true is not (should fail)" <| fun _ ->
      let subject = false
      Expect.isTrue subject "I should fail because the subject is false"

    testCase "I'm skipped (should skip)" <| fun _ ->
      Tests.skiptest "Yup, waiting for a sunny day..."

    testCase "I'm always fail (should fail)" <| fun _ ->
      Tests.failtest "This was expected..."

    testCase "contains things" <| fun _ ->
      Expect.containsAll [| 2; 3; 4 |] [| 2; 4 |]
                         "This is the case; {2,3,4} contains {2,4}"

    testCase "contains things (should fail)" <| fun _ ->
      Expect.containsAll [| 2; 3; 4 |] [| 2; 4; 1 |]
                         "Expecting we have one (1) in there"

    testCase "Sometimes I want to ༼ノಠل͟ಠ༽ノ ︵ ┻━┻" <| fun _ ->
      Expect.equal "abcdëf" "abcdef" "These should equal"

    test "I am (should fail)" {
      "╰〳 ಠ 益 ಠೃ 〵╯" |> Expect.equal true false
    }*)
  ]
