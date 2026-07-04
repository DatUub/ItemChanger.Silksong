using System.Diagnostics.CodeAnalysis;
using PrepatcherPlugin;

namespace ItemChanger.Silksong.Modules.BossKillsCounter;

/// <summary>
/// Definition for a boss whose defeat is tracked by obtaining its journal entry.
/// </summary>
public class JournalEntryBossDefinition : BossDefinition
{
    /// <inheritdoc cref="JournalEntryBossDefinition" />
    /// <param name="bossName">Internal name for the journal entry.</param>
    /// <param name="maxContrib">Number of boss kills that should contribute to the count. Set > 1 to include
    /// boss refights which grant the same journal entry.</param>
    [SetsRequiredMembers]
    public JournalEntryBossDefinition(string bossName, int maxContrib = 1)
    {
        BossName = bossName;
        MaxContribution = maxContrib;
    }

    /// <inheritdoc cref="JournalEntryBossDefinition" />
    public JournalEntryBossDefinition()
    {
    }

    /// <summary>
    /// Internal name of the boss
    /// </summary>
    public required string BossName { get; init; }

    /// <summary>
    /// Maximum number of kills that this boss's journal entry can contribute to the boss kill count. Will typically
    /// equal the number of distinct fights this boss has.
    /// </summary>
    public int MaxContribution { get; init; } = 1;

    /// <inheritdoc/>
    public override int BossesKilledContribution
    {
        get
        {
            int kills = PlayerDataAccess.EnemyJournalKillData.GetKillData(BossName).Kills;
            return kills > MaxContribution ? MaxContribution : kills;
        }
    }
}