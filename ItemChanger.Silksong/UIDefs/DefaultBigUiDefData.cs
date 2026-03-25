using ItemChanger.Serialization;
using UnityEngine;

namespace ItemChanger.Silksong.UIDefs;

public class DefaultBigUiDefData
{
    public IValueProvider<Sprite>? Sprite { get; init; }

    public Dictionary<string, IValueProvider<string>> TextSetters { get; init; } = [];

    public Dictionary<string, Vector2> Offsets { get; init; } = [];

    public List<string> Deactivations { get; init; } = [];

    public string? ActionString { get; init; }
}

public class DefaultBigUiDefDataComponent : MonoBehaviour
{
    public DefaultBigUiDefData? Data { get; set; }

    public Action? Callback { get; set; }
}
