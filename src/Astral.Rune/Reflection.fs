module internal Astral.Rune.Reflection

open System

let typeIsOpenGeneric (t1: Type) (t2 : Type) =
    if  t2.IsGenericTypeDefinition |> not then false
    else
        t1.IsConstructedGenericType && t1.GetGenericTypeDefinition() = t2

let typeHasOpenGenericInterface (t1 : Type) (t2 : Type) =
    if  t2.IsGenericTypeDefinition |> not then false
    else
        t1.GetInterfaces() 
        |> Seq.map (fun p -> printfn "%A" p; p)
        |> Seq.exists (fun p -> p.IsConstructedGenericType && p.GetGenericTypeDefinition() = t2)