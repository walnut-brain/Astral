using Lawium;
using Microsoft.Extensions.Logging;

namespace Astral.Configuration.Builders
{
    public class EventEndpointBuilder<TEvent> : EndpointBuilder
    {
        internal EventEndpointBuilder(ILoggerFactory loggerFactory, LawBookBuilder bookBuilder) : base(loggerFactory,
            bookBuilder)
        {
        }
    }
}