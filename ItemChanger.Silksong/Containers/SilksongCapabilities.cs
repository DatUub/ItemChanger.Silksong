using ItemChanger.Containers;

namespace ItemChanger.Silksong.Containers;

/// <summary>
/// Flags for ItemChanger.Core- and Silksong-defined container capabilities.
/// </summary>
public static class SilksongCapabilities
{
    /// <inheritdoc cref="ContainerCapabilities.None"/>
    public const uint None = ContainerCapabilities.None;
    /// <inheritdoc cref="ContainerCapabilities.PayCosts"/>
    public const uint PayCosts = ContainerCapabilities.PayCosts;
    /// <summary>
    /// Describes a container which can trigger a scene change after giving items.
    /// </summary>
    public const uint ChangeScene = 1 << 8;
}
