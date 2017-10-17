using System;
using System.Diagnostics.CodeAnalysis;

namespace Astral.Schema
{
    internal class CallGreenElement : EndpointGreenElement
    {
        private CallGreenElement(long id, string name, string codeNameHint, string contentType,
            ExtensionCollectionGreenElement extensions, TypeOrTypeReferenceElement requestType,
            TypeOrTypeReferenceElement responseType) : base(id, name,
            codeNameHint, contentType, extensions)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            RequestType = requestType ?? throw new ArgumentNullException(nameof(requestType));
            ResponseType = responseType;
        }

        public CallGreenElement(string name, string codeNameHint, string contentType,
            ExtensionCollectionGreenElement extensions, TypeOrTypeReferenceElement requestType,
            TypeOrTypeReferenceElement responseType) : base(name, codeNameHint,
            contentType, extensions)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            RequestType = requestType ?? throw new ArgumentNullException(nameof(requestType));
            ResponseType = responseType;
        }

        public TypeOrTypeReferenceElement RequestType { get; }
        public TypeOrTypeReferenceElement ResponseType { get; }

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public CallGreenElement With(OptionalParameter<string> Name, OptionalParameter<string> CodeNameHint,
            OptionalParameter<string> ContentType, OptionalParameter<ExtensionCollectionGreenElement> Extensions,
            OptionalParameter<TypeOrTypeReferenceElement> RequestType, OptionalParameter<TypeOrTypeReferenceElement> ResponseType)
        {
            var name = Name.WhenSkipped(this.Name);
            var codeNameHint = CodeNameHint.WhenSkipped(this.CodeNameHint);
            var contentType = ContentType.WhenSkipped(this.ContentType);
            var extensions = Extensions.WhenSkipped(this.Extensions) ?? new ExtensionCollectionGreenElement(null);
            var requestType = RequestType.WhenSkipped(this.RequestType);
            var responseType = ResponseType.WhenSkipped(this.ResponseType);
            if(name != this.Name || codeNameHint != this.CodeNameHint ||
               contentType != this.ContentType || !Equals(extensions, this.Extensions) || !Equals(requestType, this.RequestType) ||
               !Equals(responseType, this.ResponseType))
                return new CallGreenElement(Id, name, codeNameHint, contentType, extensions, requestType, responseType);
            return this;
        }

        protected bool Equals(CallGreenElement other)
        {
            return base.Equals(other) && Equals(RequestType, other.RequestType) && Equals(ResponseType, other.ResponseType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((CallGreenElement) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (RequestType != null ? RequestType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ResponseType != null ? ResponseType.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}