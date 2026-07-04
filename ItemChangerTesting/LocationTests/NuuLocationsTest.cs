using Benchwarp.Data;
using ItemChanger;
using ItemChanger.Enums;
using ItemChanger.Silksong.Modules.BossKillsCounter;
using ItemChanger.Silksong.RawData;
using ItemChanger.Tags;
using PrepatcherPlugin;

namespace ItemChangerTesting.LocationTests;

internal class NuuLocationsTest : Test
{
    public override TestMetadata GetMetadata() => new()
    {
        Folder = TestFolder.LocationTests,
        MenuName = "Nuu",
        MenuDescription = "Tests giving items from both Nuu locations",
        Revision = 20260423,
    };

    public override void Setup(TestArgs args)
    {
        StartNear(SceneNames.Halfway_01, PrimitiveGateNames.left1);

        Profile.AddPlacement(Finder.GetLocation(LocationNames.Tool_Pouch__Nuu)!.Wrap()
            .Add(Finder.GetItem(ItemNames.Flea)!));

        Profile.AddPlacement(Finder.GetLocation(LocationNames.Hunter_s_Memento)!.Wrap()
            .Add(Finder.GetItem(ItemNames.Surgeon_s_Key)!.WithTag(new PersistentItemTag()
                { Persistence = Persistence.Persistent })));
    }

    protected override void OnEnterGame()
    {
        // Need act 3 for memento check
        PlayerDataAccess.blackThreadWorld = true;
        PlayerDataAccess.act3_enclaveWakeSceneCompleted = true;
        PlayerDataAccess.act3_wokeUp = true;
    }
    
    public override IEnumerable<(string, Action)> TestMethods()
    {
        yield return ("Grant 3 Boss Entries", () => GrantBossEntries(3));
        yield return ("Grant 10 Boss Entries", () => GrantBossEntries(10));
    }

    private void GrantBossEntries(int killsToAdd)
    {
        var mod = Modules.GetOrAdd<BossKillsCounterModule>();
        
        foreach (var boss in mod.BossDefinitions)
        {
            if (boss is not JournalEntryBossDefinition entryDefinition)
                continue;
            var killData = PlayerDataAccess.EnemyJournalKillData.GetKillData(entryDefinition.BossName);
            if (killData.Kills > 0)
                continue;
            killData.Kills += 1;
            PlayerDataAccess.EnemyJournalKillData.RecordKillData(entryDefinition.BossName, killData);
            killsToAdd--;
            if (killsToAdd == 0)
                return;
        }
    }
}