using System;
using System.Net.Mime;

namespace Astral.Markup
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property)]
    public class ContentTypeAttribute : Attribute
    {
        public ContentTypeAttribute(string contentType)
        {
            ContentType = new ContentType(contentType);
        }

        public ContentType ContentType { get; }
    }
}