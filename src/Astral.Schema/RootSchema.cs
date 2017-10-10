using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Astral.Markup;
using Astral.Markup.RabbitMq;
using Astral.Schema.Data;
using Astral.Schema.RabbitMq;

namespace Astral.Schema
{
    public class RootSchema : SchemaBase<RootSchema>, IServiceSchema
    {
        

        public RootSchema(string name, string owner) 
        {
            Name = name;
            Owner = owner;
        }

        private RootSchema(string name, string owner, IReadOnlyDictionary<string, object> store) : base(store)
        {
            Name = name;
            Owner = owner;
        }


        
        
        public override RootSchema SetProperty(string property, object value)
            => new RootSchema(Name, Owner, SetParameter(property, value));
        
        public string Name { get; }
        public string Owner { get; }
        
        public string CodeName() => TryGetParameter<string>(nameof(CodeName)).IfNoneDefault();
        public RootSchema CodeName(string value) => SetProperty(nameof(CodeName), value);
        
        public Type ServiceType() => TryGetParameter<Type>(nameof(ServiceType)).IfNoneDefault();
        public RootSchema ServiceType(Type value) => SetProperty(nameof(ServiceType), value);
        
        
        
    }
}