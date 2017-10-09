using System.Collections.Generic;

namespace Astral.Schema
{
    public abstract class EndpointSchema<T> : SchemaBase<T>
        where T : EndpointSchema<T>
    {
        protected EndpointSchema(RootSchema service, string name)
        {
            Service = service;
            Name = name;
        }

        protected EndpointSchema(RootSchema service, string name, IReadOnlyDictionary<string, object> store) : base(store)
        {
            Service = service;
            Name = name;
        }

        public RootSchema Service { get; }
        public string Name { get; }
        public string CodeName() => TryGetProperty<string>(nameof(CodeName)).IfNoneDefault();
        public T CodeName(string value) => SetProperty(nameof(CodeName), value);
    }
}