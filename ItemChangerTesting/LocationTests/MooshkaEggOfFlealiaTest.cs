using Benchwarp.Data;
using ItemChanger.Silksong.RawData;
using PrepatcherPlugin;

namespace ItemChangerTesting.LocationTests;

internal class MooshkaEggOfFlealiaTest : Test
{
    public override TestMetadata GetMetadata() => new()
    {
        Folder = TestFolder.LocationTests,
        MenuName = "Mooshka Egg of Flealia",
        MenuDescription = "Tests giving an item from Mooshka's Egg of Flealia / Flea Charm award (all fleas rescued).",
        Revision = 2026051000,
    };

    public override void Setup(TestArgs args)
    {
        StartNear(SceneNames.Aqueduct_05, PrimitiveGateNames.left1);
        Profile.AddPlacement(Finder.GetLocation(LocationNames.Egg_of_Flealia)!.Wrap()
            .Add(Finder.GetItem(ItemNames.Surgeon_s_Key)!));
    }

    public override IEnumerable<(string, Action)> TestMethods()
    {
        yield return ("Rescue all fleas + caravan to Aqueduct", () =>
        {
            MooshkaTestHelpers.SetVanillaFleaCount(MooshkaTestHelpers.VanillaFleaPDBools.Length);
            PlayerDataAccess.CaravanTroupeLocation = GlobalEnums.CaravanTroupeLocations.Aqueduct;
        });
    }
}
