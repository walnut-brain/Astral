namespace rec Astral.Schema.Ast

(*
[<AbstractClass>]
type RedNode<'t when 't :> IGreenNode>(green : 't) = 
    member __.Green = green
    
    
module RedNode =
    let green<'t, 'n  when 'n :> IGreenNode and 't :> RedNode<'n>> (a : 't) =
        a.Green 
    let replace  oldNode newNode curNode =
        let g1 = green oldNode
        let g2 = green newNode
        Replacer.replaceNode g1 g2 curNode

    




type Schema(schema : Green.Schema) = 
    interface IRedNode with
        member __.ReplaceNode (oldNode : IRedNode, newNode : IRedNode) =
            RedNode.replace oldNode newNode (__.Green)
*)