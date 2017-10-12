using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Astral
{
    public abstract class BuilderBase
    {
        private IReadOnlyDictionary<string, object> Store { get; }
        protected ISet<string> ReadOnlyProps { get; }

        protected BuilderBase(IEnumerable<string> readonlyProperties, IReadOnlyDictionary<string, object> store)
        {
            if (readonlyProperties == null) throw new ArgumentNullException(nameof(readonlyProperties));
            ReadOnlyProps = new SortedSet<string>(readonlyProperties.Distinct());
            Store = store ?? throw new ArgumentNullException(nameof(store));
        }
        
        

        protected BuilderBase()
        {
            Store = new ReadOnlyDictionary<string, object>(new Dictionary<string, object>());
            ReadOnlyProps = new SortedSet<string>();
        }


        protected Option<T> TryGetParameter<T>(string name) =>
            Store.TryGetValue(name, out var obj) ? obj.ToOption().OfType<T>() : Option.None;

        protected T GetParameter<T>(string name, T defaults) => TryGetParameter<T>(name).IfNone(defaults);
        
        protected T GetParameter<T>(string name, Func<T> defaultFactory) => TryGetParameter<T>(name).IfNone(defaultFactory);

        protected IReadOnlyDictionary<string, object> SetParameter<T>(string name, T value)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if(ReadOnlyProps.Contains(name))
                throw new ArgumentException($"Property {name} declared as readonly");
            var dict = Store.ToDictionary(p => p.Key, p => p.Value);
            dict[name] = value;
            return new ReadOnlyDictionary<string, object>(dict);
        }
    }
}