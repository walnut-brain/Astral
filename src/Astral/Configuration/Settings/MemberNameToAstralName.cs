using System;
using LanguageExt;

namespace Astral.Configuration.Settings
{
    public class MemberNameToAstralName : NewType<MemberNameToAstralName, Func<string, bool, string>>
    {
        public MemberNameToAstralName(Func<string, bool,  string> value) : base(value)
        {
        }
    }
}