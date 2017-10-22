namespace Astral.Schema.Ast
open System

module Green =
    open Values
    
    type TypeOrTypeName =
        | Type of DataType
        | Name of QualifiedIdentifier 
    and ArrayType =
        | Array of TypeOrTypeName
        interface IReplacer with    
            member __.ReplaceNode(oldNode, newNode) = __ :> IReplacer
        interface IGreenNode 
    and MayBeType = 
        | Maybe of TypeOrTypeName
        interface IReplacer with    
            member __.ReplaceNode(oldNode, newNode) = __ :> IReplacer
        interface IGreenNode 
    and EnumField = 
        | EnumField of Identifier * OrdinalValue
        interface IReplacer with    
            member __.ReplaceNode(oldNode, newNode) = __ :> IReplacer
        interface IGreenNode 
    and EnumType =  
        {
            Name : QualifiedIdentifier
            CodeHint : QualifiedIdentifier option
            Type : Type option
            BaseType : OrdinalType
            IsFlags : bool
            Values : EnumField list        
        } 
        interface IReplacer with    
            member __.ReplaceNode(oldNode, newNode) = 
                let n = __.Values |> Replacer.replaceAll oldNode newNode
                if n = __.Values then __ :> IReplacer else {__ with Values = n } :> IReplacer
        interface IGreenNode 
    and TypeField = 
        | TypeField of Identifier * TypeOrTypeName
        interface IReplacer with    
            member __.ReplaceNode(oldNode, newNode) = __ :> IReplacer
        interface IGreenNode 
    and MapTypeField = 
        | MapTypeFeild of TypeField * Option<int>
        interface IReplacer with    
            member __.ReplaceNode(oldNode, newNode) = __ :> IReplacer
        interface IGreenNode 
    and MapType =
        {
            Name : QualifiedIdentifier
            CodeHint : QualifiedIdentifier option
            Type : Type option
            IsStruct : bool
            Fields : MapTypeField list
        }
        interface IReplacer with    
            member __.ReplaceNode(oldNode, newNode) = 
                let n = __.Fields |> Replacer.replaceAll oldNode newNode
                if n = __.Fields then __ :> IReplacer else {__ with Fields = n } :> IReplacer
        interface IGreenNode
        
    and OneOfType =
        {
            Name : QualifiedIdentifier option
            CodeHint : QualifiedIdentifier option
            Type : Type option
            Variants : TypeField list
        }
        interface IReplacer with    
            member __.ReplaceNode(oldNode, newNode) = 
                let n = __.Variants |> Replacer.replaceAll oldNode newNode
                if n = __.Variants then __ :> IReplacer else {__ with Variants = n } :> IReplacer
        interface IGreenNode
    and DeclareType =
        | Map of MapType
        | Enum of EnumType
        | OneOf of OneOfType
        interface IReplacer with    
            member __.ReplaceNode(oldNode, newNode) = 
                let n = 
                    match __ with
                    | Map m -> 
                        let n1 = Replacer.replaceNode oldNode newNode m
                        if n1 = m then __ else Map n1
                    | Enum e ->
                        let n1 = Replacer.replaceNode oldNode newNode e
                        if n1 = e then __ else Enum n1
                    | OneOf o ->
                        let n1 = Replacer.replaceNode oldNode newNode o
                        if n1 = o then __ else OneOf n1
                n :> IReplacer
        interface IGreenNode
    and DataType = 
        | Simple of BasicType 
        | Array of ArrayType
        | MayBe of MayBeType
        | Enum of EnumType
        | Map of MapType
        | OneOf of OneOfType
        interface IReplacer with    
            member __.ReplaceNode(oldNode, newNode) = 
                let n = 
                    match __ with
                    | Simple s ->
                        let n1 = Replacer.replaceNode oldNode newNode s
                        if n1 = s then __ else Simple s    
                    | Array a ->
                        let n1 = Replacer.replaceNode oldNode newNode a
                        if n1 = a then __ else Array a    
                    | MayBe mb ->
                        let n1 = Replacer.replaceNode oldNode newNode mb
                        if n1 = mb then __ else MayBe mb    
                    | Map m -> 
                        let n1 = Replacer.replaceNode oldNode newNode m
                        if n1 = m then __ else Map n1
                    | Enum e ->
                        let n1 = Replacer.replaceNode oldNode newNode e
                        if n1 = e then __ else Enum n1
                    | OneOf o ->
                        let n1 = Replacer.replaceNode oldNode newNode o
                        if n1 = o then __ else OneOf n1
                n :> IReplacer
        interface IGreenNode
        

    type Extension = QualifiedIdentifier * DataValue
    type ContentType = ContentType of string

    type Event =
        {
            Name : Identifier
            ContentType : ContentType option
            CodeHint : Identifier option
            EventType : TypeOrTypeName
            Extensions : Extension list
        }
    and Call =
        {
            Name : Identifier
            ContentType : ContentType option
            CodeHint : Identifier option
            RequestType : TypeOrTypeName
            ResponseType : TypeOrTypeName option
            Extensions : Extension list
        }
    and Endpoint =
        | Event of Event
        | Call of Call
    and Service =
        {
            Name : Identifier
            ContentType : ContentType option
            CodeHint : Identifier option
            Extensions : Extension list
            Endpoints : Endpoint list
        }
    and NamespaceElement =
        | Service of Service
        | DeclareType of DeclareType

    type Namespace = QualifiedIdentifier * List<NamespaceElement>

    type Schema = Namespace list

    
        