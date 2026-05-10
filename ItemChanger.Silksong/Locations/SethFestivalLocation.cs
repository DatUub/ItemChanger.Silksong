using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Locations;
using Silksong.FsmUtil;

namespace ItemChanger.Silksong.Locations;

// Shrine Guardian Seth's Guardian's Memento grant at the festival, after
// Hornet strictly beats Seth in all three flea minigames. Vanilla grants
// the Memento via CollectableItemCollect on the "Award Memento?" state.
// Strip it and run GiveAll instead. The preceding "Memento" state's
// RunDialogue + SetBoolValue stays untouched so the dialogue still plays
// and the wish-complete flag still flips.
// Verified from aqueduct_05_festival.bundle: object "Seth Sit NPC Fleatopia",
// FSM "Dialogue", state "Award Memento?".
public class SethFestivalLocation : AutoLocation
{
    protected override void DoLoad()
    {
        Using(new FsmEditGroup()
        {
            { new(SceneName!, "Seth Sit NPC Fleatopia", "Dialogue"), HookSeth },
        });
    }

    protected override void DoUnload() { }

    private void HookSeth(PlayMakerFSM fsm)
    {
        FsmState awardState = fsm.MustGetState("Award Memento?");
        awardState.RemoveActionsOfType<CollectableItemCollect>();
        awardState.InsertLambdaMethod(0, GiveAll);
    }
}
