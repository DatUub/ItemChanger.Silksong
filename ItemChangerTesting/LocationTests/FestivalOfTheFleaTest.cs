using Benchwarp.Data;
using ItemChanger;
using ItemChanger.Items;
using ItemChanger.Locations;
using ItemChanger.Serialization;
using ItemChanger.Silksong.RawData;
using ItemChanger.Silksong.Tags;
using ItemChanger.Silksong.UIDefs;
using ItemChanger.Tags;
using PrepatcherPlugin;

namespace ItemChangerTesting.LocationTests;

internal class FestivalOfTheFleaTest : Test
{
    public override TestMetadata GetMetadata() => new()
    {
        Folder = TestFolder.LocationTests,
        MenuName = "Festival of the Flea locations",
        MenuDescription = "Pale Oil (Festival Pinata) and Guardian's Memento (Seth) slots. Pick up the 'Festival on' shiny in Aqueduct_03 to force the festival state, warp to Aqueduct_05_festival, then strike the pinata / beat Seth.",
        Revision = 2026051002,
    };

    public override void Setup(TestArgs args)
    {
        StartNear(SceneNames.Aqueduct_05, PrimitiveGateNames.left1);

        Profile.AddPlacement(Finder.GetLocation(LocationNames.Pale_Oil__Fleatopia)!.Wrap()
            .Add(Finder.GetItem(ItemNames.Surgeon_s_Key)!)
            .Add(Finder.GetItem(ItemNames.Flea)!));

        Profile.AddPlacement(Finder.GetLocation(LocationNames.Guardian_s_Memento)!.Wrap()
            .Add(Finder.GetItem(ItemNames.Mask_Shard)!)
            .Add(Finder.GetItem(ItemNames.Flea)!));

        Profile.AddPlacement(Finder.GetLocation(LocationNames.Start)!.Wrap()
            .Add(Finder.GetItem(ItemNames.Swift_Step)!)
            .Add(Finder.GetItem(ItemNames.Needolin)!)
            .Add(Finder.GetItem(ItemNames.Cling_Grip)!)
            .Add(Finder.GetItem(ItemNames.Clawline)!)
            .Add(Finder.GetItem(ItemNames.Faydown_Cloak)!)
            );

        // Mirror FleatopiaTabletTest: drop a "Festival on" shiny in Aqueduct_03 so we can flip
        // FleaGamesStarted / CaravanTroupeLocation on demand and warp into Aqueduct_05_festival.
        Profile.AddPlacement(new CoordinateLocation()
        {
            SceneName = SceneNames.Aqueduct_03,
            X = 240,
            Y = 14.83f,
            Name = "Festival on",
            Managed = false
        }.Wrap()
        .WithTag(new PlacementItemsHintBoxTag())
        .Add(new SetFestivalStateItem()
        {
            Name = "Festival on",
            UIDef = new MsgUIDef() { Name = new BoxedString { Value = "Festival on" }, Sprite = new EmptySprite() }
        }.WithTag(new PersistentItemTag() { Persistence = ItemChanger.Enums.Persistence.Persistent })));
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

    private class SetFestivalStateItem : Item
    {
        public override void GiveImmediate(GiveInfo info)
        {
            PlayerDataAccess.CaravanTroupeLocation = GlobalEnums.CaravanTroupeLocations.Aqueduct;
            PlayerDataAccess.FleaGamesCanStart = true;
            PlayerDataAccess.FleaGamesStarted = true;
        }
    }
}
