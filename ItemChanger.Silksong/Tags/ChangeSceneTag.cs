using ItemChanger.Containers;
using ItemChanger.Silksong.Containers;
using ItemChanger.Tags;
using ItemChanger.Tags.Constraints;

namespace ItemChanger.Silksong.Tags;

[PlacementTag]
[LocationTag]
public class ChangeSceneTag : Tag, INeedsContainerCapability
{
    uint INeedsContainerCapability.RequestedCapabilities => SilksongCapabilities.ChangeScene;
    public required string TargetScene { get; init; }
    public required string TargetGate { get; init; }
}
