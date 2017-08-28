using System;
using Lawium;
using Microsoft.Extensions.Logging;

namespace Astral.Configuration.Builders
{
    public class CallEndpointBuilder<TMessage> : CallEndpointBuilderBase<TMessage, ValueTuple>
    {
        internal CallEndpointBuilder(ILoggerFactory loggerFactory, LawBookBuilder bookBuilder) : base(loggerFactory, bookBuilder)
        {
        }
    }
    
    
    
    public class CallEndpointBuilder<TArgs, TResult> : CallEndpointBuilderBase<TArgs, TResult>
    {
        internal CallEndpointBuilder(ILoggerFactory loggerFactory, LawBookBuilder bookBuilder) : base(loggerFactory, bookBuilder)
        {
        }
    }
}