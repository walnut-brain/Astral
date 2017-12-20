﻿using System;

namespace Astral.Payloads.DataContracts.Exceptions
{
    public class TypeEncodingException : PayloadException
    {
        public TypeEncodingException()
        {
        }

        public TypeEncodingException(string message) : base(message)
        {
        }

        public TypeEncodingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}