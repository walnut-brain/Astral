#r "../src/Astral.Rune/bin/Debug/net47/Astral.Rune.dll"
open System
module ``Sample:Test`` =
    type TaskState =
        | Created = 1
    type TaskChanged = {
        TaskId : int
        State : TaskState
    }
    type ISampleService =
        abstract TaskChanged : EventHandler<TaskChanged>

open Astral.Rune.Dsl

schema "123" {
    service ofType<ISampleService> <| is {
        contentType "text/json"
    }    
}

contentType "text/json"
contentType "text/json"




