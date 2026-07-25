using ItemChanger.Modules;
using ItemChanger.Tags;

namespace ItemChanger.Silksong.Tags;
/// <inheritdoc/>
public class RequiredModuleTag<T> : RequiredModuleTag where T : Module, new()
{
    public override Type GetModuleType() => typeof(T);
}

/// <summary>
/// Tag which ensures that a module of the specified type exists, creating it if missing.
/// </summary>
public abstract class RequiredModuleTag : Tag
{
    /// <summary>
    /// The type of module required.
    /// </summary>
    public abstract Type GetModuleType();

    protected override void DoLoad(TaggableObject parent)
    {
        base.DoLoad(parent);
        ActiveProfile!.Modules.GetOrAdd(GetModuleType());
    }
}
