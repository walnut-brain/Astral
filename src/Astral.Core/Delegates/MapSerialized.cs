using System.Net.Mime;

namespace Astral.Core
{
    public delegate (ContentType, TTarget) MapSerialized<in TSource, TTarget>(ContentType contentType, TSource source);

}