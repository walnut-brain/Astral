using System.Collections.Generic;

namespace Astral.Schema
{
    public abstract class SchemaBase<T> : BuilderBase
        where T : SchemaBase<T>
    {
        protected SchemaBase(IReadOnlyDictionary<string, object> store) : base(store)
        {
        }

        protected SchemaBase()
        {
        }
        
        public Option<TValue> TryGetProperty<TValue>(string name) =>
            TryGetParameter<TValue>(name);

        public abstract T SetProperty(string name, object value);

    }
}