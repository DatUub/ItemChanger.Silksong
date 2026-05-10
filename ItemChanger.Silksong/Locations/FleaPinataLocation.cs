using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Locations;
using QuestPlaymakerActions;
using Silksong.FsmUtil;

namespace ItemChanger.Silksong.Locations;

// Festival Pinata 10th-strike Pale Oil grant in Aqueduct_05_festival.
// Vanilla: strikes 1-9 spawn Rosaries / Shell Shards, strike 10 runs the
// "Ecstasy of the End" quest reward and spawns Pale Oil. We strip the
// reward-grant actions on the 10th-strike state and drop in GiveAll.
// EndQuest stays in whatever sibling state runs it so the wish resolves.
// TODO: confirm — GameObject path "Pinata", FSM name "Control", and
// reward state name "Reward" are best-guesses inferred from CreigeLocation
// / PinstressLocation. Needs Apollo runtime FSM dump.
public class FleaPinataLocation : AutoLocation
{
    protected override void DoLoad()
    {
        Using(new FsmEditGroup()
        {
            { new(SceneName!, "Pinata", "Control"), HookPinata },
        });
    }

    protected override void DoUnload() { }

    private void HookPinata(PlayMakerFSM fsm)
    {
        FsmState rewardState = fsm.MustGetState("Reward");
        rewardState.RemoveActionsOfType<GetQuestReward>();
        rewardState.RemoveActionsOfType<SavedItemGetV2>();
        rewardState.RemoveActionsOfType<CollectableItemGetDataV2>();
        rewardState.InsertLambdaMethod(0, GiveAll);
    }
}
