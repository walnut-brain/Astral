using System;

namespace Astral.Schema
{
    public interface ICallSchema : IEndpointSchema
    {
        Type RequestType();
        Type ResponseType();
    }
}