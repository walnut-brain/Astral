using System;
using System.Collections.Generic;
using FunEx.Monads;
using Microsoft.Extensions.Logging;

namespace Astral.Payloads.DataContracts
{
    public delegate Option<string> Encode(ILogger logger, Type type);

    public delegate Option<string> EncodeComplex(ILogger logger, Encode encode, Type type);

    public delegate Option<Type> Decode(ILogger logger, string code, IReadOnlyCollection<Type> awaited);

    public delegate Option<Type> DecodeComplex(ILogger logger, Decode decode, string code, IReadOnlyCollection<Type> awaited);
}