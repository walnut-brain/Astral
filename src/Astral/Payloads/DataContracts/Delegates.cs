using System;
using System.Collections.Generic;
using Astral.Fun.Monads;

namespace Astral.Payloads.DataContracts
{
    public delegate Option<string> Encode(ITracer logger, Type type);

    public delegate Option<string> EncodeComplex(ITracer logger, Encode encode, Type type);

    public delegate Option<Type> Decode(ITracer logger, string code, IReadOnlyCollection<Type> awaited);

    public delegate Option<Type> DecodeComplex(ITracer logger, Decode decode, string code, IReadOnlyCollection<Type> awaited);
}