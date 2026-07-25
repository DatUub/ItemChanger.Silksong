using ItemChanger.Serialization;

namespace ItemChanger.Silksong.Serialization;

/// <summary>
/// Value provider which falls back to a second value provider if the first value provider outputs null.
/// </summary>
public record CoalescingValueProvider<TValue>(IValueProvider<TValue?> First, IValueProvider<TValue> Second) : IValueProvider<TValue> where TValue : struct
{
    public TValue Value => First.Value ?? Second.Value;
}