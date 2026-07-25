using ItemChanger.Serialization;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace ItemChanger.Silksong.Modules.BossKillsCounter;

public class SpecialBossCounter : IBossCounter
{
    public SpecialBossCounter() { }

    [SetsRequiredMembers]
    public SpecialBossCounter(string name, params IEnumerable<IValueProvider<bool>> bools)
    {
        Name = name;
        BossKillTrackers = new([.. bools]);
    }

    public required string Name { get; init; }
    public required ReadOnlyCollection<IValueProvider<bool>> BossKillTrackers { get; init; }
    public int GetKillCount() => BossKillTrackers.Count(b => b.Value);
}
