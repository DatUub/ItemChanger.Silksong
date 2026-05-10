using HarmonyLib;
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
/// FSM is matched by content (state writing QuillState == Act), not by
/// FsmId tuple -- the FSM's exact name and which GameObject under
/// "Desk Inspect and Quill" hosts it aren't documented in scene dumps.
/// We piggyback on the host's PlayMakerFSM.Start prefix dispatch by
/// registering against every (scene, *, *) tuple via a small Harmony
/// patch local to this class.
/// </summary>
public class TrobbioDeskLocation : AutoLocation
{
    /// <summary>2 for Red Quill (act 2), 3 for Purple Quill (act 3).</summary>
    public int Act { get; init; }

    private static readonly List<TrobbioDeskLocation> active = [];
    private static Harmony? harmony;

    private bool hooked;

    protected override void DoLoad()
    {
        lock (active)
        {
            active.Add(this);
            if (harmony == null)
            {
                harmony = new Harmony("io.github.silksong.itemchanger.trobbiodesk");
                harmony.Patch(
                    typeof(PlayMakerFSM).GetMethod(nameof(PlayMakerFSM.Start)),
                    prefix: new HarmonyMethod(typeof(TrobbioDeskLocation).GetMethod(
                        nameof(OnFsmStart),
                        System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)));
            }
        }
    }

    protected override void DoUnload()
    {
        lock (active)
        {
            active.Remove(this);
            hooked = false;
            if (active.Count == 0 && harmony != null)
            {
                harmony.UnpatchSelf();
                harmony = null;
            }
        }
    }

    private static void OnFsmStart(PlayMakerFSM __instance)
    {
        string sceneName = __instance.gameObject.scene.name;
        lock (active)
        {
            foreach (TrobbioDeskLocation loc in active)
            {
                if (loc.hooked) continue;
                if (loc.SceneName != sceneName) continue;
                FsmState? grantState = loc.FindGrantState(__instance);
                if (grantState == null) continue;
                ItemChangerPlugin.Instance.Logger.LogInfo(
                    $"TrobbioDeskLocation Act {loc.Act}: matched FSM '{__instance.FsmName}' on '{__instance.gameObject.name}' state '{grantState.Name}'");
                loc.ApplyEdit(grantState);
                loc.hooked = true;
            }
        }
    }

    private FsmState? FindGrantState(PlayMakerFSM fsm)
    {
        foreach (FsmState state in fsm.FsmStates)
        {
            foreach (FsmStateAction action in state.Actions)
            {
                if (action is HutongGames.PlayMaker.Actions.SetPlayerDataInt spdi
                    && spdi.intName?.Value == nameof(PlayerData.QuillState)
                    && spdi.value?.Value == Act)
                {
                    return state;
                }
            }
        }
        return null;
    }

    private void ApplyEdit(FsmState grantState)
    {
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
