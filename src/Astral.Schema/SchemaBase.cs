using System.Collections.Generic;

namespace Astral.Schema
{
    public abstract class SchemaBase<T> : BuilderBase, ISchema
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

        bool ISchema.TryGetProperty<TValue>(string property, out TValue value)
        {
            var val = default(TValue);
            var result = TryGetProperty<TValue>(property).Match(p =>
            {
                val = p;
                return true;
            }, () => false);
            value = val;
            return result;
        }
    }
}