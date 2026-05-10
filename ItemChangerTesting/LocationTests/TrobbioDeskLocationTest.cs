using Benchwarp.Data;
using ItemChanger;
using ItemChanger.Silksong.Extensions;
using ItemChanger.Silksong.RawData;

namespace ItemChangerTesting.LocationTests;

internal class TrobbioDeskLocationTest : Test
{
    public override TestMetadata GetMetadata() => new()
    {
        Folder = TestFolder.LocationTests,
        MenuName = "Trobbio Desk (Quill + Notes)",
        MenuDescription = "Tests the act-2/act-3 quill and tablet split at the Library_13b desk. Inspect Region Act 2/3 grants the placed tablet item; the desk FSM grants the placed quill item.",
        Revision = 2026051000,
    };

    public override void Setup(TestArgs args)
    {
        StartNear(SceneNames.Library_13b, PrimitiveGateNames.left1);

        Profile.AddPlacement(Finder.GetLocation(LocationNames.Quill__Red)!.Wrap()
            .Add(Finder.GetItem(ItemNames.Surgeon_s_Key)!)
            .Add(Finder.GetItem(ItemNames.Flea)!));

        Profile.AddPlacement(Finder.GetLocation(LocationNames.Quill__Purple)!.Wrap()
            .Add(Finder.GetItem(ItemNames.Mask_Shard)!)
            .Add(Finder.GetItem(ItemNames.Flea)!));

        Profile.AddPlacement(Finder.GetLocation(LocationNames.Lore_Tablet__Trobbio_Notes)!.Wrap()
            .Add(Finder.GetItem(ItemNames.Spool_Fragment)!));

        Profile.AddPlacement(Finder.GetLocation(LocationNames.Lore_Tablet__Tormented_Trobbio_Notes)!.Wrap()
            .Add(Finder.GetItem(ItemNames.Spool_Fragment)!));
    }

    protected override void OnEnterGame()
    {
        base.OnEnterGame();

        var pd = PlayerData.instance;
        if (pd == null) return;

        pd.SetInt(nameof(pd.nailUpgrades), 4);
        pd.SetInt(nameof(pd.maxHealthBase), 99);
        pd.SetInt(nameof(pd.maxHealth), 99);
        pd.SetInt(nameof(pd.health), 99);
        pd.SetInt(nameof(pd.geo), 9999);

        pd.SetBool(nameof(pd.hasBrolly), true);
        pd.SetBool(nameof(pd.hasChargeSlash), true);
        pd.SetBool(nameof(pd.hasDash), true);
        pd.SetBool(nameof(pd.hasDoubleJump), true);
        pd.SetBool(nameof(pd.hasHarpoonDash), true);
        pd.SetBool(nameof(pd.hasNeedolin), true);
        pd.SetBool(nameof(pd.hasNeedolinMemoryPowerup), true);
        pd.SetBool("hasSilkSpoolAppeared", true);
        pd.SetBool(nameof(pd.hasSuperJump), true);
        pd.SetBool(nameof(pd.hasWalljump), true);
    }
}
