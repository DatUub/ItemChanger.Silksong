using Benchwarp.Data;
using ItemChanger;
using ItemChanger.Silksong.Modules.BossKillsCounter;
using ItemChanger.Silksong.RawData;
using PrepatcherPlugin;

namespace ItemChangerTesting.LocationTests;

internal class NuuLocationsTest : Test
{
    public override TestMetadata GetMetadata() => new()
    {
        Folder = TestFolder.LocationTests,
        MenuName = "Nuu",
        MenuDescription = "Tests giving items from all four Nuu locations",
        Revision = 20260423,
    };

    public override void Setup(TestArgs args)
    {
        StartNear(SceneNames.Halfway_01, PrimitiveGateNames.left1);

        Profile.AddPlacement(Finder.GetLocation(LocationNames.Hunter_s_Journal)!.Wrap()
            .WithDebugItem()
            .WithAllPersistent()
            );

        Profile.AddPlacement(Finder.GetLocation(LocationNames.Lore_Tablet__Nuu_Scroll)!.Wrap()
            .WithDebugItem()
            .WithAllPersistent()
            );

        Profile.AddPlacement(Finder.GetLocation(LocationNames.Tool_Pouch__Nuu)!.Wrap()
            .WithDebugItem()
            .WithAllPersistent()
            );

        Profile.AddPlacement(Finder.GetLocation(LocationNames.Hunter_s_Memento)!.Wrap()
            .WithDebugItem()
            .WithAllPersistent()
            );
    }

    protected override void OnEnterGame()
    {
        // Need act 3 for memento check
        //PlayerDataAccess.blackThreadWorld = true;
        //PlayerDataAccess.act3_enclaveWakeSceneCompleted = true;
        //PlayerDataAccess.act3_wokeUp = true;
        //PlayerDataAccess.nuuMementoAwarded = true;
    }
    
    public override IEnumerable<(string, Action)> TestMethods()
    {
        yield return ("Start Act 3", StartAct3);
        yield return ("Grant 3 Boss Entries", () => GrantBossEntries(3));
        yield return ("Grant 10 Boss Entries", () => GrantBossEntries(10));
    }

    private void GrantBossEntries(int killsToAdd)
    {
        var mod = Modules.GetOrAdd<BossKillsCounterModule>();
        
        foreach (var boss in mod.BossCounters.Values)
        {
            if (boss is not JournalBossCounter counter)
                continue;
            var killData = PlayerDataAccess.EnemyJournalKillData.GetKillData(counter.Name);
            if (killData.Kills > 0)
                continue;
            killData.Kills += 1;
            PlayerDataAccess.EnemyJournalKillData.RecordKillData(counter.Name, killData);
            killsToAdd--;
            if (killsToAdd == 0)
                return;
        }
    }
}