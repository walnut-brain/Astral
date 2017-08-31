using System;
using LanguageExt;

namespace Astral.Core
{
    public interface ITypeToContract
    {
        Option<string> TryMap(Type type);
    }
}