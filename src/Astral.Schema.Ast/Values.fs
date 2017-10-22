namespace Astral.Schema.Ast
open System

module Values =
    

    type OrdinalValue =
        | U8Value of byte
        | I8Value of sbyte
        | U16Value of byte
        | I16Value of byte
        | U32Value of byte
        | I32Value of byte
        | U64Value of byte
        | I64Value of byte
        interface IReplacer with
            member __.ReplaceNode(_, _) = __ :> IReplacer
        interface IGreenNode

    type BasicValue =
        | OrdinalValue of OrdinalValue
        | F32Value of float32
        | F64Value of float
        | StringValue of string
        | UuidValue of Guid
        | DTValue of DateTime
        | DTOValue of DateTimeOffset
        | TSValue of TimeSpan
        interface IReplacer with
            member __.ReplaceNode(_, _) = __ :> IReplacer
        interface IGreenNode
    and ArrayValue =
        | ArrayValue of DataValue list
        interface IReplacer with
            member __.ReplaceNode(oldNode, newNode) =
                let (ArrayValue oldAr) = __
                let newAr = oldAr |> Replacer.replaceAll oldNode newNode 
                if newAr = oldAr then __ :> IReplacer else (ArrayValue newAr) :> IReplacer
                    
    and MapValueElement = 
        | MapValueElement of string * DataValue
        interface IReplacer with
            member __.ReplaceNode(oldNode, newNode) =
                let (MapValueElement (name, curVal)) = __
                let n = Replacer.replaceSingle oldNode newNode curVal  
                if n = curVal then __ :> IReplacer else MapValueElement(name, n) :> IReplacer
                (*let (MapValueElement (name, curVal)) = __
                let valNew =
                    match oldNode with
                    | :? DataValue as oldVal ->
                        if oldVal = curVal then
                            match newNode with
                            | :? DataValue as newVal ->
                                if newVal = curVal then curVal else newVal  
                            | _ -> (curVal :> IReplacer).ReplaceNode(oldNode, newNode) :?> DataValue
                        else
                            (curVal :> IReplacer).ReplaceNode(oldNode, newNode) :?> DataValue
                    | _ -> (curVal :> IReplacer).ReplaceNode(oldNode, newNode) :?> DataValue       
                if valNew = curVal then __ :> IReplacer else (name,valNew) |> MapValueElement :> IReplacer  *) 
    and MapValue =
        | MapValue of MapValueElement list 
        interface IReplacer with
            member __.ReplaceNode(oldNode, newNode) =
               let (MapValue cur) = __
               let n = cur |> Replacer.replaceAll oldNode newNode
               if n = cur then __ :> IReplacer else (MapValue n) :> IReplacer
    and DataValue =
        | Basic of BasicValue   
        | Array of ArrayValue
        | Map of MapValue 
        interface IReplacer with
            member __.ReplaceNode(oldNode, newNode) =
                let n1 =
                    match __ with
                    | Basic b -> 
                        let n = Replacer.replaceSingle oldNode newNode b
                        if b = n then __  else Basic n 
                    | Array ar -> 
                        let n = Replacer.replaceSingle oldNode newNode ar
                        if ar = n then __ else Array n 
                    | Map map -> 
                        let n = Replacer.replaceSingle oldNode newNode map
                        if map = n then __ else Map n 
                n1 :> IReplacer