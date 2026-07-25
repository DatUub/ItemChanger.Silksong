using PrepatcherPlugin;
using System.Diagnostics.CodeAnalysis;

namespace ItemChanger.Silksong.Modules.BossKillsCounter;

/// <summary>
/// Definition for a boss whose defeat is tracked by obtaining its journal entry.
/// </summary>
public class JournalBossCounter : IBossCounter
{
    /// <inheritdoc cref="JournalBossCounter" />
    public JournalBossCounter() { }
    /// <inheritdoc cref="JournalBossCounter" />
    /// <param name="name">Internal name for the journal entry.</param>
    /// <param name="maxContribution">Number of boss kills that should contribute to the count. Set > 1 to include
    /// boss refights which grant the same journal entry.</param>
    [SetsRequiredMembers]
    public JournalBossCounter(string name, int maxContribution = 1)
    {
        Name = name;
        MaxContribution = maxContribution;
    }

    public required string Name { get; init; }
    /// <summary>
    /// Maximum number of kills that this boss's journal entry can contribute to the boss kill count. Will typically
    /// equal the number of distinct fights this boss has.
    /// </summary>
    public int MaxContribution { get; init; } = 1;
    public int GetKillCount() => Math.Min(PlayerDataAccess.EnemyJournalKillData.GetKillData(Name).Kills, MaxContribution);
}
