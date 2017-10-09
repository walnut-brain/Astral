using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RabbitLink.Builders;

namespace RabbitLink.Services
{
    public abstract class BuilderBase
    {
        private readonly IReadOnlyDictionary<string, object> _store;

        protected BuilderBase(IReadOnlyDictionary<string, object> store)
        {
            _store = store;
        }

        
        protected BuilderBase()
        {
            _store = new ReadOnlyDictionary<string, object>(new Dictionary<string, object>());
        }

        protected T GetValue<T>(string name, T defaults) => _store.TryGetValue(name, out var obj) ? (T) obj : defaults;

        protected IReadOnlyDictionary<string, object> SetValue<T>(string name, T value)
            => 
                _store.ContainsKey(name)
                    ? new ReadOnlyDictionary<string, object>(_store.Select(p => p.Key == name ? new KeyValuePair<string, object>(name, value) : p).ToDictionary(p => p.Key, p => p.Value))
                    : new ReadOnlyDictionary<string, object>(_store.Union(new [] { new KeyValuePair<string, object>(name, value) }).ToDictionary(p => p.Key, p => p.Value));
        
    }
}