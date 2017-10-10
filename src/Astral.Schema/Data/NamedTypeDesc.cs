namespace Astral.Schema.Data
{
    public abstract class NamedTypeDesc : TypeDesc
    {
        protected NamedTypeDesc(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}