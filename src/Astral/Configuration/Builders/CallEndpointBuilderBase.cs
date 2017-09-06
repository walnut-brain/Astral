using System;
using Lawium;
using Microsoft.Extensions.Logging;

namespace Astral.Configuration.Builders
{
    public class CallEndpointBuilderBase<TArgs, TResult> : EndpointBuilder
    {
        public CallEndpointBuilderBase(IServiceProvider provider, LawBookBuilder bookBuilder) : base(provider, bookBuilder)
        {
        }
    }
}