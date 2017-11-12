open System
open System.Reflection
open FSharp.Quotations
open FSharp.Quotations.Patterns
open FSharp.Quotations.DerivedPatterns
open FSharp.Quotations.ExprShape

module Test =
    let a = 1


let filterModules (typ: TypeInfo) =
    let attr = typ.GetCustomAttribute<CompilationMappingAttribute>();
    if isNull (box attr) |> not 
        then
            (attr.SourceConstructFlags &&& SourceConstructFlags.Module) = SourceConstructFlags.Module 
        else false


let displayAttr (typ : TypeInfo) =
    printfn "%s" typ.Name
    typ.GetCustomAttributes() |> Seq.map (fun p -> p.GetType().Name) |> Seq.iter (printfn "    %A")

let assembly = Assembly.GetExecutingAssembly()
assembly.DefinedTypes |> Seq.filter (fun p -> p.IsClass && (filterModules p) && not(p.Name.StartsWith("FSI"))) |> Seq.iter displayAttr


type 

module 


type FormalDescr =
    abstract Event1 : EventHandler<string>
    abstract Call : int -> unit
    abstract Call1 : a: int -> b: int -> int 



// one argument
//let (Lambda (_, Lambda (_, Call(_, v, _)))) = exp1;

// two argument
//let (Lambda (_, Lambda (_, Lambda(_, Call(_, v, _))))) = exp2;