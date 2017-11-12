module Astral.Rune.Dsl

open System
open System.Reflection
open Ast



let isModule (t : TypeInfo) =
    if t.IsClass then
        let attr = t.GetCustomAttribute<CompilationMappingAttribute>();
        match box attr with
        | null -> false
        | _ -> (attr.SourceConstructFlags &&& SourceConstructFlags.Module) = SourceConstructFlags.Module
    else
        false        

let private filterSchemaModules (asm : Assembly) =
    let filterType t = isModule t &&  (not (t.Name.StartsWith "FSI"))
    asm.DefinedTypes |> Seq.filter filterType |> Seq.toList

exception SchemaModuleNotDetectedException of string
exception ServiceNotDetectedException of string
exception UnknownServiceException of string

type ServiceProto = {
    Type : Type
    Extensions : Map<QualifiedIdentifier, Literal>
}

type SchemaProto =  {
        Version : string;
        Namespace : QualifiedIdentifier;
        Services : Map<Identifier, ServiceProto>
        Extensions : Map<QualifiedIdentifier, Literal>  
}

let private createServiceName (name: string) =
    if name.Length > 1 then
        if name.StartsWith("I") && Char.IsUpper(name.[1]) then
            Identifier.Create(name.Substring(1))
        else
            Identifier.Create(name)
    else
        Identifier.Create(name) 


let private getServices (t : TypeInfo) =
    t.DeclaredNestedTypes 
        |> Seq.filter (fun p -> p.IsInterface) 
        |> Seq.toList


       

let jsonContent =             
    fun proto ->
      { proto with 
            ServiceProto.Extensions = 
                proto.Extensions 
                    |> Map.add (Identifier.Create("contentType") |> Simple) (StringLiteral "text/json" |> BasicLiteral) 
      }  


let ofType<'t> proto =
    let name = typeof<'t>.Name |> createServiceName 
    match proto.Services |> Map.tryFind name with
    | Some s -> name, s
    | None -> sprintf "Unknown shcema service %s" typeof<'t>.Name |> UnknownServiceException |> raise 

type SchemaBuilder(proto : SchemaProto) =
    member __.Yield(()) = proto
    
    member __.Zero() = proto
    
    [<CustomOperation("jsonContentType")>]
    member __.JsonContentType (source : SchemaProto) =
        { source with Extensions = source.Extensions |> Map.add (Identifier.Create("contentType") |> Simple) (StringLiteral "text/json" |> BasicLiteral) } 
     
     
    [<CustomOperation("service")>]    
    member __.Service (source : SchemaProto, selector , applyers) =
       let (name, sproto) = selector source
       let sproto = applyers |> List.fold (fun st f -> f(st)) sproto
       { source with Services = source.Services |> Map.add name sproto }
           
     


let schema version =
    let assembly = Assembly.GetCallingAssembly()
    let schemaModule = 
        match filterSchemaModules assembly with
        | [] -> SchemaModuleNotDetectedException "Not schema module found" |> raise
        | _::_ ::_-> SchemaModuleNotDetectedException "To many schema module found" |> raise
        | [t] -> t 
    printfn "Found schema module: %s" schemaModule.Name
    let services = getServices schemaModule
    if List.isEmpty services then ServiceNotDetectedException "No service declaration found" |> raise
    services |> List.map (fun p -> p.Name) |> List.iter (printfn "Found service : %s")
    SchemaBuilder(
        {
            Version = version
            Namespace = QualifiedIdentifier.Create schemaModule.Name
            Services = services |> List.map (fun p -> (createServiceName p.Name, { Type = p; Extensions = Map.empty })) |> Map.ofList
            Extensions = Map.empty
        })




        


