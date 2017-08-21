using System;

namespace Astral.Configuration
{
    public interface IReciveExceptionPolicy
    {
        Acknowledge WhenException(Exception exception);
    }
}