using Benchwarp.Data;
using ItemChanger.Silksong.RawData;

namespace ItemChangerTesting.LocationTests;

internal class MooshkaSpoolTest : Test
{
    public override TestMetadata GetMetadata() => new()
    {
        Folder = TestFolder.LocationTests,
        MenuName = "Mooshka Spool (Judge)",
        MenuDescription = "Tests giving an item from Mooshka's Spool Fragment award at the Blasted Steps caravan stop.",
        Revision = 2026051000,
    };

    public override void Setup(TestArgs args)
    {
        StartNear(SceneNames.Coral_Judge_Arena, PrimitiveGateNames.left1);
        Profile.AddPlacement(Finder.GetLocation(LocationNames.Spool_Fragment__Flea_Caravan)!.Wrap()
            .Add(Finder.GetItem(ItemNames.Surgeon_s_Key)!));
    }

    public override IEnumerable<(string, Action)> TestMethods()
    {
        yield return ("Rescue 12 fleas + clear Judge", () => MooshkaTestHelpers.SetVanillaFleaCount(12));
    }
}
