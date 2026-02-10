using ItemChanger.Items;

namespace ItemChanger.Silksong.Items
{
    public class JournalItem : Item
    {
        public required string EnemyName { get; init; }

        public override void GiveImmediate(GiveInfo info)
        {
            var killData = PlayerData.instance.EnemyJournalKillData;
            if (killData == null) return;

            foreach (var entry in killData.list)
            {
                if (entry.Name != EnemyName) continue;
                var record = entry.Record;
                record.Kills = 1;
                record.HasBeenSeen = true;
                entry.Record = record;
                return;
            }
        }

        public override bool Redundant()
        {
            var killData = PlayerData.instance.EnemyJournalKillData;
            if (killData == null) return false;

            foreach (var entry in killData.list)
            {
                if (entry.Name != EnemyName) continue;
                return entry.Record.HasBeenSeen;
            }

            return false;
        }
    }
}
