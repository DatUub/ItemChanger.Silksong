using ItemChanger.Silksong.RawData;
using PrepatcherPlugin;

namespace ItemChanger.Silksong.Modules.BossKillsCounter;

public class LaceBossDefinition : BossDefinition
{
    public override int BossesKilledContribution {
        get
        {
            int kills = PlayerDataAccess.EnemyJournalKillData.GetKillData(JournalEntries.Lace).Kills;
            if (kills >= 2)
            {
                // Lace defeated in the Cradle, grant both kills
                return 2;
            }

            if (PlayerDataAccess.laceLeftDocks)
            {
                // Lace has left Deep Docks, the fight has been skipped
                // - this player data is set once Lace is encountered in Blasted Steps/Sinners' Road
                return kills + 1;
            }

            return kills;
        }
    }
}