using System;
using System.Collections.Generic;

namespace Astral.Schema
{
    public class CallSchema : EndpointSchema<CallSchema>, ICallSchema
    {
        public CallSchema(RootSchema service, string name) : base(service, name)
        {
        }

        private CallSchema(RootSchema service, string name, IReadOnlyDictionary<string, object> store) : base(service, name, store)
        {
        }

        public override CallSchema SetProperty(string name, object value)
            => new CallSchema(Service, Name, SetParameter(name, value));

        public Type RequestType() => TryGetProperty<Type>(nameof(RequestType)).IfNoneDefault();
        public CallSchema RequestType(Type value) => SetProperty(nameof(RequestType), value);

        public Type ResponseType() => TryGetProperty<Type>(nameof(ResponseType)).IfNoneDefault();
        public CallSchema ResponseType(Type value) => SetProperty(nameof(ResponseType), value);
    }
}