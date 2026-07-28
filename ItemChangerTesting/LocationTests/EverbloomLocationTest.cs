using ItemChanger.Silksong.RawData;
using PrepatcherPlugin;

namespace ItemChangerTesting.LocationTests;

internal class EverbloomLocationTest : Test
{
    public override TestMetadata GetMetadata() => new()
    {
        Folder = TestFolder.LocationTests,
        MenuName = "Everbloom Location",
        MenuDescription = "Tests replacing the Everbloom pickup in the Red Memory.",
        Revision = 2026070200,
    };

    protected override void OnEnterGame()
    {
        PlayerDataAccess.silkRegenMax = 3;
        PlayerDataAccess.hasBrolly = true;
        PlayerDataAccess.hasDash = true;
        PlayerDataAccess.hasDoubleJump = true;
        PlayerDataAccess.hasHarpoonDash = true;
        PlayerDataAccess.hasSuperJump = true;
        PlayerDataAccess.hasWalljump = true;
        StartAct3();
        QuestUtil.SetCompleted(Quests.Diving_Bell_Pt1_Inspect);
        PlayerDataAccess.completedMemory_shaman = true;
        CollectableItemManager.GetItemByName("Hunter Heart").AddAmount(1);
        CollectableItemManager.GetItemByName("Flower Heart").AddAmount(1);
        CollectableItemManager.GetItemByName("Coral Heart").AddAmount(1);
        QuestUtil.SetCompleted(Quests.Black_Thread_Pt5_Heart);
    }

    public override void Setup(TestArgs args)
    {
        StartAt(Benchwarp.Data.BaseBenchList.SnailShamans);
        Profile.AddPlacement(Finder.GetLocation(LocationNames.Everbloom)!.Wrap().Add(
            Finder.GetItem(ItemNames.Surgeon_s_Key)!));
    }

    public override IEnumerable<(string, Action)> TestMethods() => [("Complete Memory", () => PlayerDataAccess.CompletedRedMemory = true)];
}
