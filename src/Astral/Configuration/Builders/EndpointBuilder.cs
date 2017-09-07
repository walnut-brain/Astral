using System;
using Lawium;
using Microsoft.Extensions.Logging;

namespace Astral.Configuration.Builders
{
    public abstract class EndpointBuilder : BuilderBase
    {
        protected EndpointBuilder(LawBookBuilder<Fact> bookBuilder) : base(bookBuilder)
        {
        }
    }
}