using ItemChanger.Silksong.RawData;
using PrepatcherPlugin;

namespace ItemChanger.Silksong.Modules.BossKillsCounter;

/// <summary>
/// Treats Palestag as defeated once access to Verdania is restricted by Clover Dancers being defeated. <br />
/// TODO - if Verdania access is repeatable, this class is unnecessary and should be replaced in
///  <see cref="BossKillsCounterModule"/> by a <see cref="JournalEntryBossDefinition"/>
/// </summary>
public class PalestagBossDefinition : BossDefinition
{
    public override int BossesKilledContribution
    {
        get
        {
            int kills = PlayerDataAccess.EnemyJournalKillData.GetKillData(JournalEntries.Cloverstag_White).Kills;
            return PlayerDataAccess.defeatedCloverDancers ? 1
                : kills >= 1 ? 1
                : 0;
        }
    }
}