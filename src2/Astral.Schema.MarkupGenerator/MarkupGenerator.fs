module Astral.Schema.MarkupGenerator
open System
open System.Reflection
open System.Collections.Generic
open Astral.Markup
open Astral.Schema



type Type with
    member __.tryGetAttribute<'t when 't :> Attribute and 't : null> () =
         let attr = __.GetCustomAttribute<'t>()
         Option.ofObj attr
         
type PropertyInfo with
    member __.tryGetAttribute<'t when 't :> Attribute and 't : null> () =
         let attr = __.GetCustomAttribute<'t>()
         Option.ofObj attr         
         
module Result =
    let fromOption err opt =
        match opt with
        | Some s -> Ok s
        | _ -> Error err
          

type MarkupGeneratorError =
    | BadServiceType of string
    | CannotDetermineOwner
    | InvalidProperty of propName : string * error : string
    | InvalidMember of memberName : string * error : string
    | ExtenderPropertyError of extenderName : string * propName : string * error : string 
    | ExtenderServiceError of extenderName : string * error : string
    | UnknownType of Type

let getServiceOwner (serviceType: Type) =
    let ownerFromNamespace () =
       let nso = Option.ofObj serviceType.Namespace
       match nso with
       | Some ns ->  
            let lastDot = ns.LastIndexOf(".")
            if lastDot < 0 then Some ns else ns.Substring(lastDot + 1) |> Some
       | None -> None   
    serviceType.tryGetAttribute<OwnerAttribute>() 
        |> Option.map (fun at -> at.OwnerName)  
        |> Option.orElseWith ownerFromNamespace
        |> Option.bind (fun s -> if String.IsNullOrWhiteSpace(s) then None else Some s)
        |> Result.fromOption CannotDetermineOwner
        
        
let extendOption options extenders  =
    let folder opt (name, extender) =
        opt |> Result.bind (fun a -> extender a |> Result.mapError (fun e -> (name,e))) 
    extenders |> Seq.fold folder (Ok options)
    
             
 
let getServiceName (serviceType : Type) =
    let fromName () =
        let name = serviceType.Name
        if name.StartsWith("I") then name.Substring(1) else name
    match serviceType.tryGetAttribute<SchemaNameAttribute> () with
    | Some n -> n.Name
    | None -> fromName()
    
let (|IsEvent|_|) (prop: PropertyInfo) =
    let propType = prop.PropertyType
    if propType.IsConstructedGenericType && propType.GetGenericTypeDefinition() = typedefof<EventHandler<_>>
        then propType.GenericTypeArguments.[0] |> Some
        else None
        
let (|IsCall|_|) (prop: PropertyInfo) =
    let propType = prop.PropertyType
    if propType.IsConstructedGenericType |> not
        then None
        else
            let typeDef = propType.GetGenericTypeDefinition()
            if typeDef = typedefof<Action<_>> 
                then (propType.GenericTypeArguments.[0], None) |> Some
                else
                    if typeDef = typedefof<Func<_, _>>
                        then (propType.GenericTypeArguments.[0], propType.GenericTypeArguments.[1] |> Some) |> Some
                        else None  

let getEndpointName (prop : PropertyInfo) =
    prop.tryGetAttribute<SchemaNameAttribute>()
        |> Option.map (fun a -> a.Name)
        |> Option.defaultWith (fun () -> prop.Name)
               
                                  

let createEndpoint (extenders : ISchemaExtender list) lazyType (prop : PropertyInfo)  =
    match prop with
    | IsEvent eventType ->
        let name = getEndpointName prop
        let contentType = prop.tryGetAttribute<ContentTypeAttribute>() |> Option.map (fun a -> a.ContentType)
        let optionsRes = 
            extenders 
            |> Seq.map (fun a -> a.Name, fun opt -> a.ExtendEventByProperty(prop, opt))
            |> extendOption Map.empty 
        match optionsRes with
        | Error (name, er) -> ExtenderPropertyError(name, prop.Name, er) |> Error
        | Ok options ->
            let et =
                { 
                    EventType.CodeHint = prop.Name |> SimpleName |> Some
                    ContentType = contentType 
                    Options = options 
                    EventType.EventType = Lazy<DataType>(fun () -> lazyType eventType)
                } 
            Ok (Event(et), [eventType])    
    | IsCall (requestType, responseType) ->
        let name = getEndpointName prop
        let contentType = prop.tryGetAttribute<ContentTypeAttribute>() |> Option.map (fun a -> a.ContentType)
        let optionsRes = 
            extenders 
            |> Seq.map (fun a -> a.Name, fun opt -> a.ExtendCallByProperty(prop, opt))
            |> extendOption Map.empty 
        match optionsRes with
        | Error (name, er) -> ExtenderPropertyError(name, prop.Name, er) |> Error
        | Ok options ->
            let ct =
                { 
                    CallType.CodeHint = prop.Name |> SimpleName |> Some
                    ContentType = contentType 
                    Options = options 
                    RequestType = Lazy<DataType>(fun () -> lazyType requestType)
                    ResponseType = responseType |> Option.map (fun t -> Lazy<DataType>(fun () -> lazyType t))
                } 
            Ok  (Call(ct), responseType |> Option.map (fun rt -> [requestType; rt]) |> Option.defaultValue [requestType] ) 
    | _ -> InvalidProperty(prop.Name, "Unknown type of property") |> Error
           
           
let createEndpoints (extenders : ISchemaExtender list) lazyType (serviceType : Type) =
    let processMember state (memb : MemberInfo) =
        match state with
        | Ok(eps, types) ->
            match memb with
            | :? PropertyInfo as pi ->
                match createEndpoint extenders lazyType pi with
                | Ok (ep, t) -> Ok(ep :: eps, t @ types)  
                | Error e -> Error e
            | _ -> InvalidMember(memb.Name, "Unknown mebmer") |> Error
        | _ -> state
    serviceType.GetMembers() |> Seq.fold processMember (Ok([], []))                   


    
let (|IsArray|_|) (t:Type) =
    if t.IsArray then t.GetElementType() |> Some else None

let (|IsEnumerable|_|) (t:Type) =
    let isEnum (p : Type) = 
        p.IsConstructedGenericType && p.GetGenericTypeDefinition() = typedefof<IEnumerable<_>>
    t.GetInterfaces() 
        |> Seq.tryFind isEnum 
        |> Option.map (fun i -> i.GenericTypeArguments.[0])
        
let (|IsArrayType|_|) (t:Type) =
    match t with
    | IsArray et -> et |> Some
    | IsEnumerable et -> et |> Some
    | _ -> None 
    
let (|IsNullable|_|) (t:Type) =
    if t.IsConstructedGenericType && t.GetGenericTypeDefinition() = typedefof<Nullable<_>> 
        then t.GenericTypeArguments.[0] |> Some
        else None
        
let (|IsOption|_|) (t:Type) =
    if t.IsConstructedGenericType && t.GetGenericTypeDefinition() = typedefof<Option<_>> 
      then t.GenericTypeArguments.[0] |> Some
      else None 
    
    
let rec genType (dict : IDictionary<Type, DataType>) typ : Result<unit, MarkupGeneratorError> =
    match dict.TryGetValue(typ) with
    | (true, dt) -> Ok()
    | _ -> 
        match WellKnownType.TryResolve typ with
        | Some t -> dict.Add(typ, DataType.WellKnown t); Ok()
        | _ -> 
            match typ with
            | IsArrayType et -> 
                dict.Add(typ, ArrayType.Array (Lazy<DataType> (fun () -> dict.[et])) |> DataType.Array); Ok()
            | _ -> 
                match typ with 
                | IsNullable et ->
                    dict.Add(typ, OptionType.Option (Lazy<DataType> (fun () -> dict.[et])) |> DataType.Option); Ok()
                | IsOption et ->
                    dict.Add(typ, OptionType.Option (Lazy<DataType> (fun () -> dict.[et])) |> DataType.Option); Ok()
                | _ -> UnknownType typ |> Error
                     
let genTypes dict types =
    let folder st typ =
        match st with
        | Error _ -> st
        | Ok _ -> genType dict typ 
    types |> List.fold folder (Ok()) |> Result.map (fun _ -> dict.Values |> Seq.toList)              

    
let generate (extenders : ISchemaExtender list) (serviceType: Type)  =
    if not serviceType.IsInterface 
        then BadServiceType (sprintf "Type of service must be interface %A" serviceType) |> Error  
        else
            let processExtenders (owner, name, contentType) =
                 extenders 
                     |> List.map (fun a -> a.Name, fun opt -> a.ExtendServiceByType(serviceType, opt)))
                     |> extendOption Map.empty
                     |> Result.mapError (fun (name, er) -> ExtenderServiceError(name, er)) 
                     |> Result.map (fun opt -> owner, name, contentType, opt)
            let dict = Dictionary<Type, DataType>()
            let findEndpoints (owner, name, contentType, options) =
                createEndpoints extenders (fun typ -> dict.[typ]) serviceType
                    |> Result.map (fun (ep, t) -> owner, name, contentType, options, ep, t)
            getServiceOwner serviceType
                 |> Result.map (fun owner -> (owner, getServiceName serviceType)) 
                 |> Result.map (fun (owner, name) -> 
                                    (owner, 
                                     name, 
                                     serviceType.tryGetAttribute<ContentTypeAttribute>() |> Option.map (fun p -> p.ContentType)))
                 |> Result.bind processExtenders
                 |> Result.bind findEndpoints
                 |> Result.bind (fun (owner, name, contentType, options, endpoints, tl) ->
                                    genTypes dict tl 
                                    |> Result.map (fun t ->
                                                        {
                                                            ServiceType.CodeHint = serviceType.Name |> Some
                                                            ServiceType.ContentType = contentType
                                                            ServiceType.Endpoints = endpoints
                                                            ServiceType.Name = name
                                                            ServiceType.Options = optioms
                                                            ServiceType.Owner = owner
                                                        })) 
                                      
                 
                     
                 

