﻿using System;

namespace Astral.Schema.Data
{
    public abstract class TypeDesc : ITypeSchema
    {
        public abstract string Contract { get; }
        public abstract Type DotNetType { get; }
    }
}