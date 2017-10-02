using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Mime;
using FunEx;
using FunEx.Monads;

namespace Astral.Payloads.Serialization
{
    public delegate IEnumerable<Serialize<T>> SerializeProvider<T>(ContentType contentType);

    public delegate Result<(ContentType, T)> Serialize<T>(object value);

    public delegate IEnumerable<Deserialize<T>> DeserializeProvider<T>(Option<ContentType> contentType);

    public delegate Result<object> Deserialize<in T>(Type toType, T data);
}