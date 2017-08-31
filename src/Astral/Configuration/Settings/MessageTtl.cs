using System;
using Astral.Markup;
using Astral.Predicates;
using Lawium;

namespace Astral.Configuration.Settings
{
    public sealed class MessageTtl : Fact<TimeSpan, PositiveTimeSpan>
    {
        public MessageTtl(TimeSpan value) : base(value)
        {
        }
    }
}