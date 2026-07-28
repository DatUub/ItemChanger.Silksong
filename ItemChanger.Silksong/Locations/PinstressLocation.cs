using ItemChanger.Locations;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using QuestPlaymakerActions;
using Silksong.FsmUtil;
using ItemChanger.Silksong.Extensions;

namespace ItemChanger.Silksong.Locations;

public class PinstressLocation : AutoLocation
{
    protected override void DoLoad()
    {
        Using(new FsmEditGroup()
        {
            { new(UnsafeSceneName, "Pinstress Control", "Control"), HookPinstressSceneControl },
            { new(UnsafeSceneName, "NPC", "NPC Control"), HookPinstressNpc },
        });
    }

    protected override void DoUnload() { }

    private void HookPinstressSceneControl(PlayMakerFSM fsm)
    {
        FsmState check = fsm.MustGetState("Check");
        check.RemoveAction(5); // GetPlayerDataBool visitedIceCore
        check.RemoveAction(5); // BoolAllTrue Quest Completed and visitedIceCore => NO PINSTRESS
        // result: NO PINSTRESS if not blackThreadWorld or quest not tracked, otherwise PINSTRESS
        // quest available after TimePasses outside Room_Pinstress w/ hasChargeSlash+blackThreadWorld
        // must accept summons in Room_Pinstress for npc to appear in Peak_07
    }

    private void HookPinstressNpc(PlayMakerFSM fsm)
    {
        FsmState rewardState = fsm.MustGetState("Reward");
        rewardState.RemoveActionsOfType<GetQuestReward>();
        rewardState.RemoveActionsOfType<SavedItemGetV2>();
        rewardState.InsertLambdaMethod(0, finish =>
        {
            DialogueBox.EndConversation(true);
            this.CreateGiveAllDelegate(fsm.transform).Invoke(finish);
        });

        // revisit after quest complete
        foreach (string stateName in (string[])["Hidden Grotto", "Complete Repeat"])
        {
            FsmState state = fsm.MustGetState(stateName);
            state.ChangeTransition("CONVO_END", "Reward");
        }
    }
}
