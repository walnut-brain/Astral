﻿using System;
using FunEx;

namespace Astral.Payloads.DataContracts
{
    public delegate Result<string> ComplexTypeToContract(Type type, TypeToContract elementResolver);
}