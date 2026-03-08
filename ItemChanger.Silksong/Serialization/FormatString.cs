using ItemChanger.Serialization;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace ItemChanger.Silksong.Serialization
{
    /// <summary>
    /// An <see cref="IValueProvider{T}"/> for composing other value providers using string.Format.
    /// </summary>
    public record FormatString : IValueProvider<string>
    {
        public required IValueProvider<string> Format { get; init; }
        public required ReadOnlyCollection<IValueProvider<object>> Args { get; init; }
        public IFormatProvider? FormatProvider { get; init; }

        public static FormatString Create(IValueProvider<string> format, params IValueProvider<object>[] args) => new() { Format = format, Args = new(args) };

        [JsonIgnore]
        public string Value
        {
            get => string.Format(provider: FormatProvider, format: Format.Value, args: [.. Args.Select(a => a.Value)]);
        }
    }
}
