using Benchwarp.Data;
using ItemChanger.Locations;
using ItemChanger.Serialization;
using ItemChanger.Silksong.Locations;
using ItemChanger.Silksong.Serialization;

namespace ItemChanger.Silksong.RawData;

internal static partial class BaseLocationList
{
    public static Location Pale_Oil__Fleatopia => new FleaPinataLocation()
    {
        SceneName = SceneNames.Aqueduct_05_festival,
        Name = LocationNames.Pale_Oil__Fleatopia,
    };

    public static Location Guardian_s_Memento => new SethFestivalLocation()
    {
        SceneName = SceneNames.Aqueduct_05_festival,
        Name = LocationNames.Guardian_s_Memento,
    };

    public static Location Hermit_s_Soul => new DualLocation()
    {
        Name = LocationNames.Hermit_s_Soul,
        TrueLocation = new CoordinateLocation()
        {
            Name = "Bell Hermit act 3 shiny",
            SceneName = SceneNames.Belltown_basement_03,
            X = 97.38f,
            Y = 102.57f,
            Managed = false,
        },
        FalseLocation = new BellHermitLocation()
        {
            SceneName = SceneNames.Belltown_basement_03,
            Name = LocationNames.Hermit_s_Soul,
        },
        Test = new Disjunction(
            new PDBool(nameof(PlayerData.blackThreadWorld)), new PDBool(nameof(PlayerData.soulSnareReady))
        ),
    };
}