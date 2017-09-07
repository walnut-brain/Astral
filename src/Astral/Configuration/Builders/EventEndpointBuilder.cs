using System;
using Lawium;
using Microsoft.Extensions.Logging;

namespace Astral.Configuration.Builders
{
    public class EventEndpointBuilder<TEvent> : EndpointBuilder
    {
        internal EventEndpointBuilder(LawBookBuilder<Fact> bookBuilder) : base(bookBuilder)
        {
        }
    }
}