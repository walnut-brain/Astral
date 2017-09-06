using System;
using Lawium;
using Microsoft.Extensions.Logging;

namespace Astral.Configuration.Builders
{
    public class EventEndpointBuilder<TEvent> : EndpointBuilder
    {
        internal EventEndpointBuilder(IServiceProvider provider, LawBookBuilder bookBuilder) : base(provider,
            bookBuilder)
        {
        }
    }
}