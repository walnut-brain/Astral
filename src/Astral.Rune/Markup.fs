module Astral.Rune.Markup
open System
open System.Reflection
open Ast
open Reflection


let Core = QualifiedIdentifier.Create("Core")


type IExtensionMember =
    abstract ExtensionName : QualifiedIdentifier with get

type ISchemaParameter =
    abstract Apply : Parameters -> Parameters 

type IServiceParameter =
    abstract Apply : Parameters -> Parameters

type EndpointSpec = 
    | EventSpec of string * Type * MemberInfo
    | CallSpec of string * Type * (string * Type) list * MemberInfo 

type IEndpointParameter =
    abstract Apply : parameters: Parameters *  spec: EndpointSpec -> Parameters
 
let private contentTypeId = QualifiedIdentifier.Create("ContentType");

[<AttributeUsage(AttributeTargets.Class ||| AttributeTargets.Interface ||| 
    AttributeTargets.Property ||| AttributeTargets.Method ||| AttributeTargets.Event)>] 
type ContentTypeAttribute(contentType) =
    member private __.Apply p =
        let (Parameters m) = p
        m |> Map.add contentTypeId (contentType |> StringLiteral |> BasicLiteral) |> Parameters
    interface ISchemaParameter with
        member __.Apply p = __.Apply p
    interface IServiceParameter with
        member __.Apply p = __.Apply p
    interface IEndpointParameter with
        member __.Apply (p, _) = __.Apply p
    


    

let isFsharpEventProperty (pi: PropertyInfo) =
    if typeIsOpenGeneric pi.PropertyType typedefof<IEvent<Handler<_>, _>> then
        EventSpec(pi.Name, pi.PropertyType.GenericTypeArguments.[1], pi :> MemberInfo) |> Some
    else None
    
let isFSharpEvent (ev: EventInfo) =
    if typeIsOpenGeneric ev.EventHandlerType typedefof<Handler<_>> then
        EventSpec(ev.Name, ev.EventHandlerType.GenericTypeArguments.[0], ev :> MemberInfo) |> Some
    else None

let isEventHandlerEvent (ev: EventInfo) =
    if typeIsOpenGeneric ev.EventHandlerType typedefof<EventHandler<_>> then
        EventSpec(ev.Name, ev.EventHandlerType.GenericTypeArguments.[0], ev :> MemberInfo) |> Some
    else None

let isCallMethod (mi: MethodInfo) =
    if mi.IsSpecialName  || mi.Name.StartsWith("add_") || mi.Name.StartsWith("remove_") then None
    else [CallSpec(mi.Name, mi.ReturnType, mi.GetParameters() |> Seq.map (fun t -> t.Name, t.ParameterType) |> Seq.toList, mi)] |> Some

let private createServiceName (name: string) =
    if name.Length > 1 then
        if name.StartsWith("I") && Char.IsUpper(name.[1]) then
            Identifier.Create(name.Substring(1))
        else
            Identifier.Create(name)
    else
        Identifier.Create(name)

module TypeManager = 
    type Manager = private | Manager
    let getReferenceFor (manager : Manager) (typ:Type)  : TypeReference = raise (NotImplementedException()) 


type ProcessServiceError =
    | ServiceAlreadyExists of string

let findEndpoints (typ : Type) =
    let filter (mi : MemberInfo) =
        match mi with
        | :? PropertyInfo as pi ->
            match isFsharpEventProperty pi with
            | Some e -> [e]
            | None -> []    
        | :? MethodInfo as mi ->
            match isCallMethod mi with
            | Some e -> [e]
            | None -> [] 
        | :? EventInfo as ei ->
            match isFSharpEvent ei |> Option.orElseWith (fun () -> isEventHandlerEvent ei) with
            | Some e -> [e]
            | _ ->[]
        | _ -> []
    typ.GetMembers() |> Seq.collect filter |> Seq.toList
    
   
let makeParameters<'i > (mi : MemberInfo) f =
    let folder (prms, ext) (attr : Attribute) =
        let ext = 
            match box attr with
            | :? IExtensionMember as em -> ext |> Set.add em.ExtensionName
            | _ -> ext
        let prms = 
            match box attr with
            | :? 'i as sp -> f sp prms
            | _ -> prms
        (prms, ext)    
    mi.GetCustomAttributes() |> Seq.fold folder (Parameters Map.empty, Set.empty)   
   
let private specToEndpoint refFromType spec  =
    match spec with
    | EventSpec (name, eType, mi) ->
        let id = Identifier.Create name
        let typeRef =  refFromType eType
        let (prms, ext) = makeParameters<IEndpointParameter> mi (fun p prms -> p.Apply (prms, spec))
        id, Event { EventType = typeRef }, prms, ext  
    | CallSpec (name, retType, args, mi) ->
        let id = Identifier.Create name
        let (prms, ext) = makeParameters<IEndpointParameter> mi (fun p prms -> p.Apply (prms, spec))
        let retRef = refFromType retType
        let args1 =
            match args with
            | [] -> Multiple Map.empty 
            | [(n, t)] -> refFromType t |> Single
            | _ -> args |> List.map (fun (n, t) -> Identifier.Create n, refFromType t) |> Map.ofList |> Multiple
        id, Call { Arguments = args1; Result = retRef }, prms, ext
         
let processService (intf : Type) typeManager schema =
    let name = createServiceName intf.Name
    if schema.Services |> Map.containsKey name then
        sprintf "Service with name %A already exists" name |> ServiceAlreadyExists |> Error
    else
        let ep = findEndpoints intf |> List.map (specToEndpoint (TypeManager.getReferenceFor typeManager))
        let (prm, ext) = makeParameters<IServiceParameter> intf (fun p prms -> p.Apply prms)
        let ext =  ext
                    |> Set.union (ep |> List.fold (fun st (_, _, _, e) -> Set.union st e) schema.Extensions) 
        { schema with
            Extensions = ext
            Services = schema.Services |> Map.add name ((ep |> List.map (fun (id, e, pr, _) -> id, (e, pr)) |> Map.ofList) , prm) 
        } |> Ok
                                     
        
        
        
