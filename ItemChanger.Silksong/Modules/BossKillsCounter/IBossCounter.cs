namespace ItemChanger.Silksong.Modules.BossKillsCounter;

/// <summary>
/// Interface used by <see cref="BossKillsCounterModule"/> for individual boss kill trackers.
/// </summary>
public interface IBossCounter
{
    /// <summary>
    /// A unique identifier for the boss: for base game bosses, the internal name for the boss.
    /// </summary>
    string Name { get; }
    /// <summary>
    /// The number of kills to provide to <see cref="BossKillsCounterModule"/>; typically the number of unique encounters defeated or otherwise cleared.
    /// </summary>
    int GetKillCount();
}
