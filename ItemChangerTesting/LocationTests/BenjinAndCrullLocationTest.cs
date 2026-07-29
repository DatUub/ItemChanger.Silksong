using Benchwarp.Data;
using ItemChanger;
using ItemChanger.Silksong.Extensions;
using ItemChanger.Silksong.Items;
using ItemChanger.Silksong.RawData;

namespace ItemChangerTesting.LocationTests;

internal class BenjinAndCrullLocationTest : Test
{
    public override TestMetadata GetMetadata() => new()
    {
        Folder = TestFolder.LocationTests,
        MenuName = "Benjin & Crull",
        MenuDescription = "Test both Benjin & Crull locations",
        Revision = 2026043000,
    };

    public override void Setup(TestArgs args)
    {
        StartNear(SceneNames.Dust_Shack, PrimitiveGateNames.left1);
        
        Profile.AddPlacement(Finder.GetLocation(LocationNames.Start)!.Wrap()
            .Add(RosariesItem.MakeRosariesItem(1000)));

        Profile.AddPlacement(Finder.GetLocation(LocationNames.Tacks)!.Wrap()
            .WithVariousItems()
            .WithAllPersistent()
            );

        Profile.AddPlacement(Finder.GetLocation(LocationNames.Steel_Spines)!.Wrap()
            .WithVariousItems()
            .WithAllPersistent()
            );
    }

    protected override void OnEnterGame()
    {
        // Tacks prerequisites
        //FullQuestBase roachQuest = QuestManager.GetQuest(Quests.Roach_Killing);
        //roachQuest.SetReadyToComplete();

        // Steel spines prerequisites
        //FullQuestBase infestationQuest = QuestManager.GetQuest(Quests.Doctor_Curse_Cure);
        //infestationQuest.SetAccepted();
    }

    public override IEnumerable<(string, Action)> TestMethods()
    {
        yield return ("Start Act 3", this.StartAct3);
        yield return ("Accept Roach Quest", () => QuestManager.GetQuest(Quests.Roach_Killing).SetAccepted());
        yield return ("Ready to Complete Roach Quest", () => QuestManager.GetQuest(Quests.Roach_Killing).SetReadyToComplete());
        yield return ("Accept Doctor Quest", () => QuestManager.GetQuest(Quests.Doctor_Curse_Cure).SetAccepted());
        yield return ("Complete Doctor Quest", () => QuestManager.GetQuest(Quests.Doctor_Curse_Cure).SetCompleted());
        yield return ("Give Steel Spines", () => Finder.GetItem(ItemNames.Steel_Spines)!.Give(null, new()));
        yield return ("Give Tacks", () => Finder.GetItem(ItemNames.Tacks)!.Give(null, new()));
    }
}