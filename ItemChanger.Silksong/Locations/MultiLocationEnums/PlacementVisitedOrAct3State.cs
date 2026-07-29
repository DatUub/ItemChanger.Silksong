using ItemChanger.Enums;
using ItemChanger.Serialization;
namespace ItemChanger.Silksong.Locations.MultiLocationEnums;

public enum PlacementVisitedOrAct3State
{
    /// <summary>
    /// Not Act3, placement does not have required VisitState.
    /// </summary>
    NotVisited,

    /// <summary>
    /// Not Act3, placement has required VisitState. Specific required flags can be configured in 
    /// <see cref="EnumValueProviders.PlacementVisitedOrAct3StateProvider(string, VisitState, IValueProvider{bool}?)"/>.
    /// </summary>
    Visited,

    /// <summary>
    /// The world is in Act 3 state; placement visit state is not considered.
    /// </summary>
    Act3
}