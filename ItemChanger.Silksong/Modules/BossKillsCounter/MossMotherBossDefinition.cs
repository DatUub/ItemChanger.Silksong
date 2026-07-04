using Benchwarp.Data;
using HutongGames.PlayMaker;
using PrepatcherPlugin;
using Silksong.FsmUtil;

namespace ItemChanger.Silksong.Modules.BossKillsCounter;

public class MossMotherBossDefinition : BossDefinition
{
    public bool HasDefeatedDoubleMossMothers { get; set; } = false;

    private static readonly FsmId MossMothersBattleEndFsmId = new(SceneNames.Weave_03, "Battle End", "Control");

    public override void DoLoad()
    {
        SilksongHost.Instance.AddFsmEdit(MossMothersBattleEndFsmId, HookMossMothersBattleEnd);
    }

    public override void DoUnload()
    {
        SilksongHost.Instance.RemoveFsmEdit(MossMothersBattleEndFsmId, HookMossMothersBattleEnd);
    }

    private void HookMossMothersBattleEnd(PlayMakerFSM fsm)
    {
        fsm.InsertMethod("Award Journal", 0, () => { HasDefeatedDoubleMossMothers = true; });
    }

    public override int BossesKilledContribution
    {
        get
        {
            // Counts the following locations as Moss Mothers boss fights:
            // - Tut_03 - tutorial boss fight in Act 1 Moss Grotto
            // - Weave_03 - double fight in Weavenest Atla

            int kills = 0;
            if (PlayerDataAccess.defeatedMossMother)
                kills++;
            if (HasDefeatedDoubleMossMothers)
                kills++;

            return kills;
        }
    }
}