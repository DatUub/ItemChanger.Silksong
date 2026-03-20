using Benchwarp.Util;
using ItemChanger.Serialization;
using Silksong.AssetHelper.ManagedAssets;

namespace ItemChanger.Silksong.Assets;

/// <summary>
/// Special case (non-generic) for GameObjects because there
/// may be special case code.
/// </summary>
internal class GameObjectCache : IObjectCache<GameObject>
{
    private Dictionary<string, ManagedAsset<GameObject>> _assets = [];

    public GameObject GetAsset(string key)
    {
        // TODO - if using something like a ManagedAssetList to load an asset, special case it here.

        if (_assets.TryGetValue(key, out ManagedAsset<GameObject> asset))
        {
            asset.EnsureLoaded();
            return asset.Handle.Result;
        }

        throw new ArgumentException($"GameObject asset with key {key} not recognized");
    }

    public GameObjectCache()
    {
        // Load scene assets
        {
            using Stream stream = GetType().Assembly.GetManifestResourceStream("ItemChanger.Silksong.Resources.Assets.scenegameobjects.json");

            Dictionary<string, ManagedAssetGroup<GameObject>.SceneAssetInfo>? data =
                SerializationHelper.DeserializeResource<Dictionary<string, ManagedAssetGroup<GameObject>.SceneAssetInfo>>(stream);

            if (data == null)
            {
                throw new ArgumentException($"Could not find scene game objects resource");
            }

            foreach ((string key, ManagedAssetGroup<GameObject>.SceneAssetInfo info) in data)
            {
                _assets[key] = ManagedAsset<GameObject>.FromSceneAsset(sceneName: info.SceneName, objPath: info.ObjPath);
            }
        }

        // Load non-scene assets
        {
            using Stream stream = GetType().Assembly.GetManifestResourceStream("ItemChanger.Silksong.Resources.Assets.nonscenegameobjects.json");

            Dictionary<string, ManagedAssetGroup<GameObject>.NonSceneAssetInfo>? data =
                SerializationHelper.DeserializeResource<Dictionary<string, ManagedAssetGroup<GameObject>.NonSceneAssetInfo>>(stream);

            if (data == null)
            {
                throw new ArgumentException($"Could not find non-scene game objects resource");
            }

            foreach ((string key, ManagedAssetGroup<GameObject>.NonSceneAssetInfo info) in data)
            {
                _assets[key] = ManagedAsset<GameObject>.FromNonSceneAsset(assetName: info.AssetName, bundleName: info.BundleName);
            }
        }
    }

    public void Load()
    {
        foreach (ManagedAsset<GameObject> asset in _assets.Values)
        {
            asset.Load();
        }
    }

    public void Unload()
    {
        foreach (ManagedAsset<GameObject> asset in _assets.Values)
        {
            asset.Unload();
        }
    }
}
