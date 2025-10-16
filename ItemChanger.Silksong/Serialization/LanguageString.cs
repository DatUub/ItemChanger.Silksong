using ItemChanger.Serialization;
using Newtonsoft.Json;
using TeamCherry.Localization;

namespace ItemChanger.Silksong.Serialization
{
    public record LanguageString(string Sheet, string Key) : IString
    {
        [JsonIgnore]
        public string Value
        {
            get => Language.Get(Key, Sheet);
        }

        IString IString.Clone() => this with { };
    }
}
