using ItemChanger.Serialization;

namespace ItemChanger.Silksong.Assets;

public class ManagedAssetProvider<T> : IValueProvider<T>
{
    public required string Key { get; init; }

    public T Value => AssetCache.GetAsset<T>(Key);
}
