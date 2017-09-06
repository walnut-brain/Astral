using System;

namespace Astral.Configuration
{
    internal static class ConfigUtils
    {
        internal static string NormalizeTag(string tag)
        {
            return String.IsNullOrWhiteSpace(tag) ? "" : tag;
        }
    }
}