using ItemChanger.Serialization;

namespace ItemChanger.Silksong.Serialization;

public record JournalKillDataKills(string JournalRecordName) : IValueProvider<int>
{
    public int Value => PlayerData.instance.EnemyJournalKillData.GetKillData(JournalRecordName).Kills;
}