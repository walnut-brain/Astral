using System.Net.Mime;

namespace Astral.Core
{
    public interface ISerializedMapper<TF1, TF2>
    {
        (ContentType, TF2) Map(ContentType contentType, TF1 value);
    }
}