using System;

namespace Astral.Markup
{
    [AttributeUsage(AttributeTargets.All)]
    public class SchemaNameAttribute : Attribute
    {
        public SchemaNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}