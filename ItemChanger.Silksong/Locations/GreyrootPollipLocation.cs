using ItemChanger.Locations;
using ItemChanger.Enums;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Silksong.FsmUtil;
using ItemChanger.Silksong.Extensions;

namespace ItemChanger.Silksong.Locations;

public class GreyrootPollipLocation : AutoLocation
{
    protected override void DoLoad()
    {
        Using(new FsmEditGroup()
        {
            {new(UnsafeSceneName, "Wood Witch", "Dialogue"), HookWitch},
        });
    }

    protected override void DoUnload() {}

    private void HookWitch(PlayMakerFSM fsm)
    {
        FsmState rewardQueryState = fsm.MustGetState("Pollip Reward?");
        int i = rewardQueryState.IndexFirstActionOfType<CheckIfToolUnlocked>();
        rewardQueryState.RemoveAction(i);
        rewardQueryState.InsertLambdaMethod(i, (finish) =>
        {
            if (!Placement!.CheckVisitedAny(VisitState.ObtainedAnyItem))
            {
                fsm.SendEvent("POLLIP REWARD");
            }
            finish();
        });
        rewardQueryState.AddLambdaMethod(this.CreateGiveAllDelegate(fsm.transform)); // only reached if POLLIP REWARD was not sent.

        FsmState rewardState = fsm.MustGetState("Flower Quest Reward");
        i = rewardState.IndexFirstActionOfType<SetToolUnlocked>();
        rewardState.RemoveAction(i);
        rewardState.InsertLambdaMethod(i, this.CreateGiveAllDelegate(fsm.transform));
    }
}