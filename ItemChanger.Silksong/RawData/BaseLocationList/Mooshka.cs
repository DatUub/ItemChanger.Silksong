using Benchwarp.Data;
using ItemChanger.Locations;
using ItemChanger.Silksong.Locations;

namespace ItemChanger.Silksong.RawData;

// Mooshka (Caravan Troupe Leader) flea-count-gated rewards. See issue #201.
// Brew refill semantics are deferred to a follow-up; Flea_Brew location is
// intentionally omitted until shop/refill support lands.

internal static partial class BaseLocationList
{
    public static Location Spool_Fragment__Flea_Caravan => new MooshkaJudgeLocation
    {
        SceneName = SceneNames.Coral_Judge_Arena,
        Name = LocationNames.Spool_Fragment__Flea_Caravan,
    };

    public static Location Tool_Pouch__Mooshka => new MooshkaFleatopiaLocation
    {
        SceneName = SceneNames.Aqueduct_05_caravan,
        Name = LocationNames.Tool_Pouch__Mooshka,
        RewardStates = ["Award Tool Pouch", "Just Gave Tool Pouch"],
    };

    public static Location Egg_of_Flealia => new MooshkaFleatopiaLocation
    {
        SceneName = SceneNames.Aqueduct_05_caravan,
        Name = LocationNames.Egg_of_Flealia,
        RewardStates = ["Award Flea Charm", "Has Flea Charm"],
    };
}
