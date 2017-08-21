using LanguageExt;

namespace Astral.Configuration.Settings
{
    public class OriginalMessage : NewType<OriginalMessage, object>
    {
        public OriginalMessage(object value) : base(value)
        {
        }
    }
}