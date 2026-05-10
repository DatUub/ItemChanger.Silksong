using Benchwarp.Data;
using ItemChanger.Silksong.RawData;
using PrepatcherPlugin;

namespace ItemChangerTesting.LocationTests;

internal class MooshkaToolPouchTest : Test
{
    public override TestMetadata GetMetadata() => new()
    {
        Folder = TestFolder.LocationTests,
        MenuName = "Mooshka Tool Pouch",
        MenuDescription = "Tests giving an item from Mooshka's Tool Pouch reward in Fleatopia.",
        Revision = 2026051000,
    };

    public override void Setup(TestArgs args)
    {
        StartNear(SceneNames.Aqueduct_05, PrimitiveGateNames.left1);
        Profile.AddPlacement(Finder.GetLocation(LocationNames.Tool_Pouch__Mooshka)!.Wrap()
            .Add(Finder.GetItem(ItemNames.Surgeon_s_Key)!));
    }

    public override IEnumerable<(string, Action)> TestMethods()
    {
        yield return ("Rescue 22 fleas + move caravan to Aqueduct", () =>
        {
            MooshkaTestHelpers.SetVanillaFleaCount(22);
            PlayerDataAccess.CaravanTroupeLocation = GlobalEnums.CaravanTroupeLocations.Aqueduct;
        });
    }
}
