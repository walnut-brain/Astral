using System;
using LanguageExt;

namespace Astral.Configuration.Settings
{
    public class ResponseType : NewType<ResponseType, Type>
    {
        public ResponseType(Type value) : base(value)
        {
        }
    }
}