using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Locations;
using Silksong.FsmUtil;

namespace ItemChanger.Silksong.Locations;

/// <summary>
/// Location for the Red Quill (Act 2) / Purple Quill (Act 3) granted by the
/// desk in Library_13b. Vanilla FSM activates a quill GameObject which gives
/// a PlayerDataCollectable setting QuillState, then the FSM re-writes
/// QuillState (to 2 or 3) and sets hasQuill. We strip those PD writes plus
/// the quill ActivateGameObject from the act-specific state and insert
/// GiveAll. Lore tablet side ("Inspect Region Act 2/3") is routed through
/// TabletContainer independently.
///
/// Per-act state is identified by the literal int value (2 or 3) written
/// to QuillState; act 2 and act 3 share one FSM with a PD branch.
/// </summary>
public class TrobbioDeskLocation : AutoLocation
{
    /// <summary>2 for Red Quill (act 2), 3 for Purple Quill (act 3).</summary>
    public int Act { get; init; }

    protected override void DoLoad()
    {
        Using(new FsmEditGroup()
        {
            { new(SceneName!, "Desk Inspect and Quill", "Desk Inspect and Quill"), HookDesk },
        });
    }

    protected override void DoUnload() { }

    private void HookDesk(PlayMakerFSM fsm)
    {
        FsmState? grantState = null;
        foreach (FsmState state in fsm.FsmStates)
        {
            foreach (FsmStateAction action in state.Actions)
            {
                if (action is HutongGames.PlayMaker.Actions.SetPlayerDataInt spdi
                    && spdi.intName?.Value == nameof(PlayerData.QuillState)
                    && spdi.value?.Value == Act)
                {
                    grantState = state;
                    break;
                }
            }
            if (grantState != null) break;
        }

        if (grantState == null) return;

        grantState.RemoveFirstActionMatching(a =>
            a is HutongGames.PlayMaker.Actions.SetPlayerDataInt s
            && s.intName?.Value == nameof(PlayerData.QuillState));
        grantState.RemoveFirstActionMatching(a =>
            a is HutongGames.PlayMaker.Actions.SetPlayerDataBool b
            && b.boolName?.Value == nameof(PlayerData.hasQuill));
        grantState.RemoveActionsOfType<ActivateGameObject>();
        grantState.InsertLambdaMethod(0, GiveAll);
    }
}
