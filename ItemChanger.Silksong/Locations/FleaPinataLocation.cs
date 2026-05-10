using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Locations;
using Silksong.FsmUtil;

namespace ItemChanger.Silksong.Locations;

// Festival Pinata 10th-strike Pale Oil grant in Aqueduct_05_festival.
// Strikes 1-9 use Money! / Reward 1 / Reward 2 to spawn rosaries + shards;
// strike 10 enters Reward Final which flings four Pale Oil pickups via
// FlingObjectsFromGlobalPoolV2. Strip the flings and grant through IC.
// Verified from aqueduct_05_festival.bundle: object "Flea Festival Pinata",
// FSM "Rewards", state "Reward Final".
public class FleaPinataLocation : AutoLocation
{
    protected override void DoLoad()
    {
        Using(new FsmEditGroup()
        {
            { new(SceneName!, "Flea Festival Pinata", "Rewards"), HookPinata },
        });
    }

    protected override void DoUnload() { }

    private void HookPinata(PlayMakerFSM fsm)
    {
        FsmState rewardFinal = fsm.MustGetState("Reward Final");
        rewardFinal.RemoveActionsOfType<FlingObjectsFromGlobalPoolV2>();
        rewardFinal.InsertLambdaMethod(0, GiveAll);
    }
}
