using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Astral.Schema
{
    public class AstNode
    {
        
    }

    public interface ITypeDeclarationNode
    {
        string TypeName { get;  }
        Option<string> TypeCode { get; }
    }

    public class TypeDeclarationNode : AstNode, ITypeDeclarationNode
    {
        public string TypeName { get; set; }
        public Option<string> TypeCode { get; set; }
    }

    public interface IPrimitiveTypeDeclarationNode : ITypeDeclarationNode
    {
        PrimitiveTypeKind Kind { get;  }
    }

    public class PrimitiveTypeDeclarationNode : TypeDeclarationNode, IPrimitiveTypeDeclarationNode
    {
        public PrimitiveTypeKind Kind { get; set; }
    }

    public interface IOptionTypeDeclarationNode : ITypeDeclarationNode
    {
        ITypeDeclarationNode ElementType { get;  }
    }

    public class OptionTypeDeclarationNode : TypeDeclarationNode, IOptionTypeDeclarationNode
    {
        public TypeDeclarationNode ElementType { get; set; }
        ITypeDeclarationNode IOptionTypeDeclarationNode.ElementType => ElementType;
    }

    public interface IArrayTypeDeclarationNode : ITypeDeclarationNode
    {
        ITypeDeclarationNode ElementType { get; }
    }

    public class ArrayTypeDeclarationNode : TypeDeclarationNode, IArrayTypeDeclarationNode
    {
        public TypeDeclarationNode ElementType { get; set; }
        ITypeDeclarationNode IArrayTypeDeclarationNode.ElementType => ElementType;
    }

    public interface IComplexTypeDeclarationNode : ITypeDeclarationNode
    {
        Option<ComplexTypeDeclarationNode> BaseOn { get;  }
        IReadOnlyDictionary<string, ITypeDeclarationNode> Fields { get;  }
    }

    public class ComplexTypeDeclarationNode : TypeDeclarationNode, IComplexTypeDeclarationNode
    {
        
        public Option<ComplexTypeDeclarationNode> BaseOn { get; set; }  
        public Dictionary<string, TypeDeclarationNode> Fields { get; set; }

        IReadOnlyDictionary<string, ITypeDeclarationNode> IComplexTypeDeclarationNode.Fields 
            => new ReadOnlyDictionary<string, ITypeDeclarationNode>(Fields.ToDictionary(p => p.Key, p => (ITypeDeclarationNode) p.Value));
    }

    public interface IEnumTypeDeclarationNode
    {
        IPrimitiveTypeDeclarationNode Base { get;  }
        IReadOnlyDictionary<string, PrimitiveTypeValue> Values { get; }
    }

    public class EnumTypeDeclarationNode : TypeDeclarationNode, IEnumTypeDeclarationNode
    {
        public PrimitiveTypeDeclarationNode Base { get; set; }
        public Dictionary<string, PrimitiveTypeValue> Values { get;  set; }

        IPrimitiveTypeDeclarationNode IEnumTypeDeclarationNode.Base => Base;

        IReadOnlyDictionary<string, PrimitiveTypeValue> IEnumTypeDeclarationNode.Values => Values;
        
    }

    public interface IUnionTypeDeclaration
    {
        Option<IComplexTypeDeclarationNode> Base { get; }
        IReadOnlyDictionary<string, ITypeDeclarationNode> Variants { get; }
    }

    public class UnionTypeDeclaration : TypeDeclarationNode, IUnionTypeDeclaration
    {
        
        public Option<ComplexTypeDeclarationNode> Base { get; set; }
        public Dictionary<string, TypeDeclarationNode> Variants { get; set; }

        Option<IComplexTypeDeclarationNode> IUnionTypeDeclaration.Base => Base.OfType<IComplexTypeDeclarationNode>();
        

        IReadOnlyDictionary<string, ITypeDeclarationNode> IUnionTypeDeclaration.Variants 
            => new ReadOnlyDictionary<string, ITypeDeclarationNode>(Variants.ToDictionary(p => p.Key, p => (ITypeDeclarationNode) p.Value)); 
    }

    public class PrimitiveTypeValue
    {
        
    }
    
    public enum PrimitiveTypeKind
    {
    }

    public class ServiceDeclarationNode : AstNode
    {
        
    }
}