using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Locations;
using QuestPlaymakerActions;
using Silksong.FsmUtil;

namespace ItemChanger.Silksong.Locations;

// Shrine Guardian Seth Guardian's Memento grant at the festival, after
// Hornet strictly beats Seth's score in all three games. Same shape as
// CreigeLocation: strip GetQuestReward + SavedItemGetV2 from the reward
// state, insert GiveAll. Gating ("strictly beat all three") lives on the
// FSM's transition into this state, so the location is naturally reachable
// only post-victory.
// Object name "Seth" / FSM "Dialogue" / state "Reward" are inferred from
// peer NPC locations; needs Apollo runtime verification.
public class SethFestivalLocation : AutoLocation
{
    protected override void DoLoad()
    {
        Using(new FsmEditGroup()
        {
            { new(SceneName!, "Seth", "Dialogue"), HookSeth },
        });
    }

    protected override void DoUnload() { }

    private void HookSeth(PlayMakerFSM fsm)
    {
        FsmState rewardState = fsm.MustGetState("Reward");
        rewardState.RemoveActionsOfType<GetQuestReward>();
        rewardState.RemoveActionsOfType<SavedItemGetV2>();
        rewardState.RemoveActionsOfType<CollectableItemGetDataV2>();
        rewardState.InsertLambdaMethod(0, GiveAll);
    }
}
