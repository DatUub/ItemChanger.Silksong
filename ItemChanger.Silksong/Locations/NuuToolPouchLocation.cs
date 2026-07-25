using HarmonyLib;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Locations;
using ItemChanger.Silksong.Extensions;
using ItemChanger.Silksong.Modules.BossKillsCounter;
using ItemChanger.Silksong.RawData;
using MonoMod.RuntimeDetour;
using Silksong.FsmUtil;

namespace ItemChanger.Silksong.Locations;

public class NuuToolPouchLocation : AutoLocation
{
    /// <summary>
    /// Number of boss kills required for completing Bugs of Pharloom wish
    /// </summary>
    public required int RequiredBossKills { get; init; }

    protected override void DoLoad()
    {
        // Override Bugs of Pharloom quest to track boss kills rather than journal entries
        Using(new Hook(
            AccessTools.Method(
                typeof(JournalQuestTarget),
                nameof(JournalQuestTarget.GetCompletionAmount)
            ), BossKillCountHook));

        // Modify the amount of boss kills required to complete the quest
        QuestManager.GetQuest(Quests.Journal).ModifyTargetAmount(RequiredBossKills);

        Using(new FsmEditGroup()
        {
            { new(UnsafeSceneName, "Nuu", "Dialogue"), HookGetQuestReward }
        });
    }

    protected override void DoUnload()
    {
    }

    private static int BossKillCountHook(
        Func<JournalQuestTarget, QuestCompletionData.Completion, int> orig,
        JournalQuestTarget self,
        QuestCompletionData.Completion sourceCompletion)
    {
        return ActiveProfile!.Modules.GetOrAdd<BossKillsCounterModule>().BossKillCount;
    }

    private void HookGetQuestReward(PlayMakerFSM fsm)
    {
        // Replace quest reward with IC placement
        FsmState getRewardState = fsm.MustGetState("Get Reward?");
        getRewardState.GetFirstActionOfType<SavedItemGet>()!.enabled = false;

        FsmState completeConvoState = fsm.AddState("Complete Convo 5");
        fsm.ChangeTransition("Complete Convo 4", "CONVO_END", "Complete Convo 5");
        fsm.AddTransition("Complete Convo 5", "FINISHED", "End Dialogue");
        completeConvoState.AddAction(new EndDialogue()
        {
            ReturnControl = false,
            ReturnHUD = false,
            Target = new FsmOwnerDefault() { OwnerOption = OwnerDefaultOption.UseOwner },
            UseChildren = false
        });
        completeConvoState.AddLambdaMethod(GiveAll);
    }
}