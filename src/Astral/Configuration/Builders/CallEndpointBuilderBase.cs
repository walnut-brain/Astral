using System;
using Lawium;
using Microsoft.Extensions.Logging;

namespace Astral.Configuration.Builders
{
    public class CallEndpointBuilderBase<TArgs, TResult> : EndpointBuilder
    {
        public CallEndpointBuilderBase(LawBookBuilder bookBuilder) : base(bookBuilder)
        {
        }
    }
}