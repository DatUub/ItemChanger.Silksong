using Benchwarp.Data;
using ItemChanger;
using ItemChanger.Silksong.RawData;
using ItemChanger.Silksong.StartDefs;

namespace ItemChangerTesting.LocationTests;

internal class MrMushroomTabletTest : Test
{
    public override TestMetadata GetMetadata() => new()
    {
        Folder = TestFolder.LocationTests,
        MenuName = "Mr Mushroom tablet",
        MenuDescription = "Place debug item at the Mr Mushroom riddle tablet in Aqueduct_05 (Putrified Ducts)",
        Revision = 2026051000,
    };

    public override void Setup(TestArgs args)
    {
        // Drop in near the tablet. Putrified Ducts secret room; rough coords
        // from bundle inspection — tweak if probe shows otherwise.
        StartAt(new CoordinateStartDef()
        {
            SceneName = SceneNames.Aqueduct_05,
            X = 70f,
            Y = 30f,
            MapZone = GlobalEnums.MapZone.NONE,
        });

        Profile.AddPlacement(Finder.GetLocation(LocationNames.Lore_Tablet__Mr_Mushroom)!.Wrap()
            .WithDebugItem(persistence: ItemChanger.Enums.Persistence.Persistent));

        // Kit needed to reach + read the tablet in Act 3.
        Profile.AddPlacement(Finder.GetLocation(LocationNames.Start)!.Wrap()
            .Add(Finder.GetItem(ItemNames.Swift_Step)!)
            .Add(Finder.GetItem(ItemNames.Cling_Grip)!)
            .Add(Finder.GetItem(ItemNames.Clawline)!)
            .Add(Finder.GetItem(ItemNames.Faydown_Cloak)!)
            .Add(Finder.GetItem(ItemNames.Silk_Soar)!)
            );
    }
}
