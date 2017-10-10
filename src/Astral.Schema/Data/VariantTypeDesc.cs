using System.Collections.Generic;

namespace Astral.Schema.Data
{
    public abstract class VariantTypeDesc : TypeDesc
    {
        protected VariantTypeDesc(IReadOnlyDictionary<string, TypeDesc> variants)
        {
            Variants = variants;
        }

        public IReadOnlyDictionary<string, TypeDesc> Variants { get; }
    }
}