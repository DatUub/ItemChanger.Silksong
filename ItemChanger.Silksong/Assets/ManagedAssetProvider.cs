using ItemChanger.Serialization;
using Newtonsoft.Json;

namespace ItemChanger.Silksong.Assets;

public class ManagedAssetProvider<T> : IValueProvider<T>
{
    public required string Key { get; init; }

    [JsonIgnore] public T Value => AssetCache.GetAsset<T>(Key);
}
