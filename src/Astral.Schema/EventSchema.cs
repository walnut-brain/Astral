using System;
using System.Collections.Generic;

namespace Astral.Schema
{
    public class EventSchema : EndpointSchema<EventSchema>
    {
        public EventSchema(RootSchema service, string name) : base(service, name)
        {
        }

        private EventSchema(RootSchema service, string name, IReadOnlyDictionary<string, object> store) : base(service, name, store)
        {
        }

        public Type ContractType() => TryGetProperty<Type>(nameof(ContractType)).IfNoneDefault();
        public EventSchema ContractType(Type value) => SetProperty(nameof(ContractType), value);

        public override EventSchema SetProperty(string name, object value)
            => new EventSchema(Service, Name, SetParameter(name, value));
    }
}