using System;
using System.Net.Mime;
using LanguageExt;

namespace Astral.Core
{
    public delegate Try<object> Deserialize<in TFormat>(Type toType, ContentType contentType, TFormat data);

}