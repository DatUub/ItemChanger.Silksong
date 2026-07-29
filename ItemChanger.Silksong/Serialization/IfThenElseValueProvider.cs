using ItemChanger.Serialization;
using ItemChanger.Silksong.Extensions;
using Newtonsoft.Json;

namespace ItemChanger.Silksong.Serialization;

[method: JsonConstructor]
public class IfThenElseValueProvider<TValue>(IValueProvider<bool> Test, IValueProvider<TValue> TrueValue, IValueProvider<TValue> FalseValue)
    : IValueProvider<TValue>
{
    public IfThenElseValueProvider(IValueProvider<bool> Test, TValue TrueValue, TValue FalseValue)
        : this(Test, TrueValue.ToValueProvider(), FalseValue.ToValueProvider()) { }

    public IfThenElseValueProvider(IValueProvider<bool> Test, TValue TrueValue, IValueProvider<TValue> FalseValue)
        : this(Test, TrueValue.ToValueProvider(), FalseValue) { }

    public IfThenElseValueProvider(IValueProvider<bool> Test, IValueProvider<TValue> TrueValue, TValue FalseValue)
        : this(Test, TrueValue, FalseValue.ToValueProvider()) { }

    [JsonIgnore] public TValue Value => Test.Value ? TrueValue.Value : FalseValue.Value;
}
