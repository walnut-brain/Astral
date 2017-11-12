open System
open System.Reflection
open FSharp.Control


[<AutoOpen>]
module ``Sample:Test`` =
    type TaskState =
        | Created = 1

    type TaskChanged = {
        TaskId : int
        State : TaskState
    }

    type ChangeTask = {
        TaskId : int
        State : TaskState
    }
    
    type ISampleService =
        //[<CLIEvent>]
        abstract TaskChanged : IEvent<TaskChanged>
        abstract ChangeTaskState : ChangeTask -> unit

type EndpointInfo = 
    | Event of string * Type * MemberInfo
    | Call of string * Type * Type list * MemberInfo 
    | Unknown of MemberInfo 

let typeIsGeneric (t1: Type) (t2 : Type) =
    t1.IsConstructedGenericType && t1.GetGenericTypeDefinition() = t2

let typeHasGenericInterface (t1 : Type) (t2 : Type) =
    t1.GetInterfaces() 
    |> Seq.map (fun p -> printfn "%A" p; p)
    |> Seq.exists (fun p -> p.IsConstructedGenericType && p.GetGenericTypeDefinition() = t2)


let isFsharpEventProperty (pi: PropertyInfo) =
    if typeIsGeneric pi.PropertyType typedefof<IEvent<Handler<_>, _>> then
        Event(pi.Name, pi.PropertyType.GenericTypeArguments.[1], pi :> MemberInfo) |> Some
    else None


let isFSharpEvent (ev: EventInfo) =
    if typeIsGeneric ev.EventHandlerType typedefof<Handler<_>> then
        Event(ev.Name, ev.EventHandlerType.GenericTypeArguments.[0], ev :> MemberInfo) |> Some
    else None

let findEndpoints (typ : Type) =
    let detectEndpoint (mi : MemberInfo) =
        match mi with
        | :? PropertyInfo as pi ->
            match isFsharpEventProperty pi with
            | Some e -> [e]
            | None -> [Unknown mi]    
        | :? MethodInfo as mi ->
            if mi.IsSpecialName  || mi.Name.StartsWith("add_") || mi.Name.StartsWith("remove_") then []
            else [Call(mi.Name, mi.ReturnType, mi.GetParameters() |> Seq.map (fun t -> t.ParameterType) |> Seq.toList, mi)]
        | :? EventInfo as ei ->
            match isFSharpEvent ei with
            | Some e -> [e]
            | _ -> 
                [Unknown mi]
        | _ -> [Unknown mi]
    typ.GetMembers() |> Seq.collect detectEndpoint |> Seq.toList

findEndpoints typeof<ISampleService>