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

    runParser (spaces >>. pCodeUnit)
        """
    namespace YouDo
    {
        type DeviceType = enum
        {
            Apple = 1;
            Android = 2;
        }
        
        type FraudStatus = enum
        {
            GrayList = 0;
            BlackList = 1;
            WhiteList = 2;
        }
        
        type GuardSensorInfo = map
        {
            Method : string;
            UserId : ?i32;
            IPAddress : string;
            FingerPrint : string;
            FingerPrintValid : bool;
            TimeStamp : utcdatetime;
            Url : string
        }
        
        type TaskEntity = map 
        {
            Id : ?int;
            CreatorId : ?int;
            DateCreation : datetime
        } 
        
        type TaskChange = map
        {
            OldTaskEntity : ?TaskEntity;
            NewTaskEntity : TaskEntity;
        }
        
        
        service Events
        {
            event SensorInfo : GuardSensorInfo 
                exchange = {
                    Name = "youdo.guard.sensor", 
                    Type = Fanout
                };
                
            event TaskChange : TaskChange
                exchage = {
                    Name = "youdo.task.state",
                    Type = Topic
                }
                routingKey = "#";
        }
        
    }
"""
     
    (* """
        open Something;
        namespace Test {
        
            service Test {
                event Formal : [] oneof 
                    {  
                        Ok; 
                        frm : u64; 
                        ffr : ?enum(u32, true) 
                                { 
                                    one = 1; 
                                    two = 2u64 
                                } 
                    } 
                exchange= -123i32 routingKey=123.45;
             
            call unit Formal(Request) exchange="123" routingKey=12345;
            }
            
            type Reqest1 = map {
                Id : i32;
                Name : string;
            }
        }""" 
     *)
    //runParser pEventParser """call oneof { Ok ; Formal: [] oneof { frm : u64; ffr : ?enum { one = 1; two = 2u64 } } exchange="123" routingKey=12345;""" 
    Tests.runTestsInAssembly defaultConfig argv
