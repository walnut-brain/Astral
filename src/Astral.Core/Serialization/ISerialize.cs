using System;
using System.Net.Mime;
using LanguageExt;

namespace Astral.Core
{
    public interface ISerialize<TFormat>
    {
        Try<(ContentType, TFormat)> Serialize(Type toType, object obj);
    }
}