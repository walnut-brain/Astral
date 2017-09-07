namespace Astral.Schema
{
    public class SchemaRecord
    {
        public SchemaRecord(string extension, string settingName, object settingValue)
        {
            Extension = extension;
            SettingName = settingName;
            SettingValue = settingValue;
        }

        public string Extension { get; }
        public string SettingName { get; }
        public object SettingValue { get; }
    }
}