using System;
using System.Collections.Generic;

namespace Astral.Schema
{
    internal abstract class EndpointGreenElement : SchemaGreenElement
    {
        protected EndpointGreenElement(long id, string name, string codeNameHint, string contentType, ExtensionCollectionGreenElement extensions) : base(id)
        {
            Name = name;
            CodeNameHint = codeNameHint;
            ContentType = contentType;
            Extensions = extensions;
        }

        protected EndpointGreenElement(string name, string codeNameHint, string contentType, ExtensionCollectionGreenElement extensions)
        {
            Name = name;
            CodeNameHint = codeNameHint;
            ContentType = contentType;
            Extensions = extensions ?? new ExtensionCollectionGreenElement(null);
        }

        public string Name { get; }
        public string CodeNameHint { get; }
        public string ContentType { get; }
        public ExtensionCollectionGreenElement Extensions { get; }

        protected override IReadOnlyCollection<SchemaGreenElement> Children => new[] {Extensions};

        protected bool Equals(EndpointGreenElement other)
        {
            return base.Equals(other) && string.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase) &&
                   string.Equals(CodeNameHint, other.CodeNameHint) &&
                   string.Equals(ContentType, other.ContentType) &&
                   Equals(Extensions, other.Extensions);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((EndpointGreenElement) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(Name) : 0);
                hashCode = (hashCode * 397) ^ (CodeNameHint != null ? CodeNameHint.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ContentType != null ? ContentType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Extensions != null ? Extensions.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ base.GetHashCode();
                return hashCode;
            }
        }
    }
}