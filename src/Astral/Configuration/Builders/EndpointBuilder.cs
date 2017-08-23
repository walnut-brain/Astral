using Astral.Lawium;
using Microsoft.Extensions.Logging;

namespace Astral.Configuration.Builders
{
    public abstract class EndpointBuilder : BuilderBase
    {
        protected EndpointBuilder(ILoggerFactory loggerFactory, LawBookBuilder bookBuilder) : base(loggerFactory, bookBuilder)
        {
        }
    }
}