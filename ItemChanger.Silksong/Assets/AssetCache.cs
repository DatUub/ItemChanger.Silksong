using UnityEngine;

namespace ItemChanger.Silksong.Assets;

/// <summary>
/// Class providing an entry point for base game assets loaded by ItemChanger via AssetHelper.
/// </summary>
public static class AssetCache
{
    private static Dictionary<Type, IObjectCache> _objectCacheLookup = [];

    internal static void Init(SilksongHost host)
    {
        host.LifecycleEvents.OnEnterGame += LoadAll;
        host.LifecycleEvents.OnLeaveGame += UnloadAll;

        /* 
         * To add a new asset:
         * If the type is listed here, simply add an entry to the corresponding file in Resources/Assets
         * If the type is not listed here, then you must do the following steps:
         * - Create a json file in Resources/Assets following the example of the sprites.json file
         * - Add a line here to register the generic object cache in the lookup
         * - Add an asset names utility class in AssetKeys.cs
         */
        
        _objectCacheLookup[typeof(Sprite)] = GenericObjectCache<Sprite>.FromEmbeddedResource("ItemChanger.Silksong.Resources.Assets.sprites.json");
        _objectCacheLookup[typeof(GameObject)] = new GameObjectCache();
    }

    public static T GetAsset<T>(this string key)
    {
        if (!_objectCacheLookup.TryGetValue(typeof(T), out IObjectCache untypedCache))
        {
            throw new ArgumentException($"No object cache found for type {typeof(T).Name}");
        }

        IObjectCache<T>? typedCache = untypedCache as IObjectCache<T>;
        if (typedCache == null)
        {
            throw new InvalidOperationException($"Could not cast object cache for type {typeof(T).Name}");
        }

        return typedCache.GetAsset(key);
    }


    private static void LoadAll()
    {
        foreach (IObjectCache cache in _objectCacheLookup.Values)
        {
            cache.Load();
        }
    }

    private static void UnloadAll()
    {
        foreach (IObjectCache cache in _objectCacheLookup.Values)
        {
            cache.Unload();
        }
    }
}
