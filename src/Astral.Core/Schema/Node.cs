using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Astral.Schema
{

    public interface IDeclarationSchema
    {
        string Name { get; }
    }
    
    public interface ITypeDeclarationSchema : IDeclarationSchema
    {
        string CodeName { get; }
        string Code { get; }
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
        Option<IComplexTypeDeclarationSchema> BaseOn { get;  }
        IReadOnlyDictionary<string, ITypeDeclarationSchema> Fields { get;  }
    }


    public interface IEnumTypeDeclarationSchema : ITypeDeclarationSchema
    {
        ITypeDeclarationSchema BaseOn { get;  }
        bool IsFlags { get; }
        IReadOnlyDictionary<string, object> Values { get; }
    }


    public interface IUnionTypeDeclarationSchema : ITypeDeclarationSchema
    {
        IReadOnlyDictionary<string, ITypeDeclarationSchema> Variants { get; }
    }

    public interface IServiceDeclarationSchema : IDeclarationSchema
    {
        
    }

    public interface IEndpointDeclarationSchema : IDeclarationSchema
    {
        
    }
    
    
}