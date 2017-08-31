using System;
using Astral.Markup;
using LanguageExt;
using Lawium;

namespace Astral.Configuration.Settings
{
    public sealed class MemberNameToAstralName : Fact<Func<string, bool, string>>
    {
        public MemberNameToAstralName(Func<string, bool, string> value) : base(value)
        {
        }
    }
}