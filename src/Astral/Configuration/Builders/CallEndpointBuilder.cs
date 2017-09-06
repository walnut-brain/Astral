using System;
using Lawium;
using Microsoft.Extensions.Logging;

namespace Astral.Configuration.Builders
{
    public class CallEndpointBuilder<TMessage> : CallEndpointBuilderBase<TMessage, ValueTuple>
    {
        internal CallEndpointBuilder(IServiceProvider provider, LawBookBuilder bookBuilder) : base(provider, bookBuilder)
        {
        }
    }
    
    
    
    public class CallEndpointBuilder<TArgs, TResult> : CallEndpointBuilderBase<TArgs, TResult>
    {
        internal CallEndpointBuilder(IServiceProvider provider, LawBookBuilder bookBuilder) : base(provider, bookBuilder)
        {
        }
    }
}