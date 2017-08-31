using System;
using System.Net.Mime;
using LanguageExt;

namespace Astral.Core
{
    public interface IDeserialize<in TFormat>
    {
        Try<object> Deserialize(Type toType, ContentType contentType, TFormat data);
    }
}