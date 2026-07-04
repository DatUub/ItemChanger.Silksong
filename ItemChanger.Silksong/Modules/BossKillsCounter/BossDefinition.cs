namespace ItemChanger.Silksong.Modules.BossKillsCounter;

/// <summary>
/// Abstract definition of a boss. Used by <see cref="BossKillsCounterModule"/> to count how many bosses have been
/// defeated.
/// </summary>
public abstract class BossDefinition
{
    /// <summary>
    /// Returns the number of boss kills contributed by this definition. Typically, this should return <c>0</c>
    /// when the boss hasn't been killed and <c>1</c> when the boss has been killed, but can be greater e.g. for
    /// boss refights.
    /// </summary>
    public abstract int BossesKilledContribution { get; }

    public virtual void DoLoad()
    {
    }

    public virtual void DoUnload()
    {
    }
}