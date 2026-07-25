using HarmonyLib;
using HutongGames.PlayMaker;
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
            { new(UnsafeSceneName, "Nuu", "Dialogue"), HookGetQuestReward },
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
        return ActiveProfile!.Modules.GetOrAdd<BossKillsCounterModule>().GetKillCount();
    }

    private void HookGetQuestReward(PlayMakerFSM fsm)
    {
        // give reward before end dialogue rather than after, so that control is relinquished.
        // on revisit, respawned items are given at the end of dialogue
        // (except when nuu permanently leaves, in which case the room must be reloaded)
        FsmState getRewardState = fsm.MustGetState("Get Reward?");
        getRewardState.Actions = [];

        FsmState endDialogue = fsm.MustGetState("End Dialogue");
        FsmState endDialogueIC = fsm.AddState("End Dialogue IC");
        endDialogueIC.AddActions(endDialogue.Actions);
        endDialogue.Actions = [];
        endDialogueIC.AddTransition("FINISHED", getRewardState.Name);
        endDialogue.ChangeTransition("FINISHED", endDialogueIC.Name);

        endDialogue.AddLambdaMethod(callback =>
        {
            if (QuestManager.GetQuest(Quests.Journal).IsCompleted)
            {
                this.CreateGiveAllDelegate(fsm.transform).Invoke(callback);
            }
            else
            {
                callback();
            }
        });
    }
}