using System;
using System.Net.Mime;
using LanguageExt;

namespace Astral.Core
{
    public delegate Try<(ContentType, TFormat)> Serialize<TFormat>(Type type, object value);

}