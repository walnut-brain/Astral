using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Astral.Schema
{

    public interface ITypeDeclarationSchema 
    {
        string ContractName { get; }
        string CodeName { get; }
        string SchemaName { get; }
        Type DotNetType { get; }
        bool IsWellKnown { get; }
    }

    

    public interface IOptionTypeDeclarationSchema : ITypeDeclarationSchema
    {
        ITypeDeclarationSchema ElementType { get;  }
    }


    public interface IArrayTypeDeclarationSchema : ITypeDeclarationSchema
    {
        ITypeDeclarationSchema ElementType { get; }
    }


    public interface IComplexTypeDeclarationSchema : ITypeDeclarationSchema
    {
        bool IsStruct { get; }
        IComplexTypeDeclarationSchema BaseOn { get;  }
        IReadOnlyDictionary<string, ITypeDeclarationSchema> Fields { get;  }
    }


    public interface IEnumTypeDeclarationSchema : ITypeDeclarationSchema
    {
        ITypeDeclarationSchema BaseOn { get;  }
        IReadOnlyDictionary<string, long> Values { get; }
        bool IsFlags { get; }
    }


    public interface IUnionTypeDeclarationSchema : ITypeDeclarationSchema
    {
        IReadOnlyDictionary<string, ITypeDeclarationSchema> Variants { get; }
    }

    public interface IServiceDeclarationSchema 
    {
        
    }

    public interface IEndpointDeclarationSchema 
    {
        
    }
    
    
}