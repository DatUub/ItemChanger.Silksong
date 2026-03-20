namespace ItemChanger.Silksong.Assets;

internal interface IObjectCache
{
    public void Load();

    public void Unload();
}

internal interface IObjectCache<T> : IObjectCache
{
    public T GetAsset(string key);
}
