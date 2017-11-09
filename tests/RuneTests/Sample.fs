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
    checkSuccess "qualified identifier" pqid "Fgh.Abcd" (Complex (Simple (createIdentifier "Fgh"), (createIdentifier "Abcd")))
    checkFail "qualified identifier" pqid "Fgh.,Abcd"
    checkSuccess "basic literal" pBasicLiteral "54" (OrdinalLiteral (I32Literal 54))
    checkSuccess "basic literal" pBasicLiteral "54u8" (OrdinalLiteral (U8Literal 54uy))
    checkSuccess "basic literal" pBasicLiteral "54i64" (OrdinalLiteral (I64Literal 54L))
    checkSuccess "basic literal" pBasicLiteral "54.f32" (F32Literal 54.f)
    checkSuccess "basic literal" pBasicLiteral "54.0" (F64Literal 54.0)
    checkSuccess "basic literal" pBasicLiteral "54.1f64" (F64Literal 54.1)
    checkSuccess "basic literal" pBasicLiteral "54." (F64Literal 54.0)
    checkSuccess "literal" pLiteral 
      """[   54.0, 
                                      "hell\"ow", 
                                      true, 
                                      none, 
                                      resample,
                                      { 
                                          v = [2u32, 7.5], 
                                          b = no 
                                      }]""" 
      (ArrayLiteral
                                         [BasicLiteral (F64Literal 54.0); BasicLiteral (StringLiteral "hell\"ow");
                                          BasicLiteral (BoolLiteral true); BasicLiteral NoneLiteral;
                                          IdentifierLiteral (Simple (createIdentifier "resample"));
                                          MapLiteral
                                            [(createIdentifier "v",
                                              ArrayLiteral
                                                [BasicLiteral (OrdinalLiteral (U32Literal 2u));
                                                 BasicLiteral (F64Literal 7.5)]);
                                             (createIdentifier "b", BasicLiteral (BoolLiteral true))]])
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
