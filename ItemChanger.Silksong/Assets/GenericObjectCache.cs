using Benchwarp.Util;
using ItemChanger.Serialization;
using Silksong.AssetHelper.ManagedAssets;

namespace ItemChanger.Silksong.Assets;

internal class GenericObjectCache<T> : IObjectCache<T>
{
    private Dictionary<string, ManagedAsset<T>> _assets = [];

    public T GetAsset(string key)
    {
        ManagedAsset<T> asset = _assets.TryGetValue(key, out ManagedAsset<T> fromDict)
            ? fromDict
            : throw new KeyNotFoundException($"Did not find key {key} for type {typeof(T).Name}");

        asset.EnsureLoaded();
        return asset.Handle.Result;
    }

    public static GenericObjectCache<T> FromEmbeddedResource(string resourceName)
    {
        GenericObjectCache<T> cache = new();

        using Stream stream = typeof(GenericObjectCache<>).Assembly.GetManifestResourceStream(resourceName);
        // Why does this accept a stream :angery:
        Dictionary<string, ManagedAssetGroup<T>.NonSceneAssetInfo>? data =
            SerializationHelper.DeserializeResource<Dictionary<string, ManagedAssetGroup<T>.NonSceneAssetInfo>>(stream);

        if (data == null)
        {
            throw new ArgumentException($"Could not find embedded resource {resourceName}");
        }

        foreach ((string key, ManagedAssetGroup<T>.NonSceneAssetInfo info) in data)
        {
            cache._assets[key] = ManagedAsset<T>.FromNonSceneAsset(assetName: info.AssetName, bundleName: info.BundleName);
        }

        return cache;
    }

    public void Load()
    {
        foreach (ManagedAsset<T> asset in _assets.Values)
        {
            asset.Load();
        }
    }

    public void Unload()
    {
        foreach (ManagedAsset<T> asset in _assets.Values)
        {
            asset.Unload();
        }
    }
}
