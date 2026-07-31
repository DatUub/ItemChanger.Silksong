using HarmonyLib;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Silksong.FsmStateActions;
using Silksong.FsmUtil;
using System.Reflection;

namespace ItemChanger.Silksong.Modules.CustomSkills;

/// <summary>
/// Novelty: Swift Step sprint while grounded only — no air sprint, no dash for logic.
///
/// flibber (#209): trick the game so the vanilla sprint path runs (animations / cState),
/// rather than faking speed in GetRunSpeed. We rewrite the sprintFSM's hasDash gate to
/// <c>hasDash || (hasGroundedSprint &amp;&amp; onGround)</c> so the FSM owns speed tiers
/// (Sprintmaster / Quickening) and anim. Direct field <c>playerData.hasDash</c> stays false
/// until real Swift Step (CanDash, etc.).
///
/// FSM viewer: HeroController GameObject → component sprintFSM (PlayMakerFSM "Sprint") →
/// states that contain PlayerDataBoolTest(boolName=hasDash). Known candidates listed in
/// <see cref="KnownHasDashGateStates"/>; discovery pass logs any additional names once.
/// </summary>
public class GroundedSprintModule : CustomSkillModule
{
#pragma warning disable IDE1006 // Naming Styles
    public bool hasGroundedSprint { get; set; }
#pragma warning restore IDE1006

    /// <summary>
    /// Sprint FSM state names that gate on hasDash (PlayerDataBoolTest).
    /// Prefer named lookup (flibber review) over scanning every state.
    /// Confirmed/extended via one-time discovery log when empty hit.
    /// </summary>
    private static readonly string[] KnownHasDashGateStates =
    [
        "Sprint?",
        "Can Sprint?",
        "Has Dash?",
        "Check Dash",
        "Check Sprint",
        "Try Sprint",
        "TRY SPRINT",
    ];

    private static GroundedSprintModule? _activeInstance;
    private static readonly FieldInfo? SprintBufferStepsField =
        AccessTools.Field(typeof(HeroController), "sprintBufferSteps");

    private Harmony? _harmony;
    private bool _fsmPatched;

    public override IEnumerable<string> GettableSkillBools() => [nameof(hasGroundedSprint)];

    public override bool GetBool(string boolName) => boolName switch
    {
        nameof(hasGroundedSprint) => hasGroundedSprint,
        _ => throw UnsupportedBoolName(boolName),
    };

    public override IEnumerable<string> SettableSkillBools() => [nameof(hasGroundedSprint)];

    public override void SetBool(string boolName, bool value)
    {
        switch (boolName)
        {
            case nameof(hasGroundedSprint):
                hasGroundedSprint = value;
                break;
            default:
                throw UnsupportedBoolName(boolName);
        }
    }

    protected override void DoLoad()
    {
        base.DoLoad();
        _activeInstance = this;
        _fsmPatched = false;

        // Patch sprintFSM once Hero is alive (sprintFSM assigned in HeroController.Start region).
        Using(Md.HeroController.Start.Postfix(OnHeroStart));

        _harmony = new Harmony("itemchanger.silksong.groundedsprint");

        MethodInfo? update = AccessTools.Method(typeof(HeroController), "Update");
        if (update != null)
        {
            _harmony.Patch(update,
                postfix: new HarmonyMethod(typeof(GroundedSprintModule), nameof(HeroUpdatePostfix)));
        }

        MethodInfo? leftGround = AccessTools.Method(typeof(HeroController), "LeftGround", [typeof(bool)]);
        if (leftGround != null)
        {
            _harmony.Patch(leftGround,
                postfix: new HarmonyMethod(typeof(GroundedSprintModule), nameof(LeftGroundPostfix)));
        }
        else
        {
            MethodInfo? becomeAirborne = AccessTools.Method(typeof(HeroController), "BecomeAirborne");
            if (becomeAirborne != null)
            {
                _harmony.Patch(becomeAirborne,
                    postfix: new HarmonyMethod(typeof(GroundedSprintModule), nameof(LeftGroundPostfix)));
            }
        }

        // Bool overload is the real body (no-arg often inlined).
        MethodInfo? heroJumpBool = AccessTools.Method(typeof(HeroController), "HeroJump", [typeof(bool)]);
        if (heroJumpBool != null)
        {
            _harmony.Patch(heroJumpBool,
                prefix: new HarmonyMethod(typeof(GroundedSprintModule), nameof(HeroJumpBoolPrefix)));
        }
    }

    protected override void DoUnload()
    {
        _harmony?.UnpatchSelf();
        _harmony = null;
        _fsmPatched = false;
        if (_activeInstance == this) _activeInstance = null;
        base.DoUnload();
    }

    private void OnHeroStart(HeroController self)
    {
        TryPatchSprintFsm(self);
    }

    /// <summary>
    /// Rewrite hasDash PlayerDataBoolTest actions on the sprint FSM so Grounded Sprint
    /// can enter the vanilla sprint path while on the ground only.
    /// </summary>
    private void TryPatchSprintFsm(HeroController self)
    {
        if (_fsmPatched) return;
        PlayMakerFSM? sprint = self.sprintFSM;
        if (sprint == null || sprint.Fsm == null) return;

        int rewrites = 0;
        var patchedStateNames = new List<string>();

        // 1) Named states first (maintainability — flibber #209).
        foreach (string stateName in KnownHasDashGateStates)
        {
            FsmState? state = sprint.GetState(stateName);
            if (state == null) continue;
            int n = RewriteHasDashTests(state);
            if (n > 0)
            {
                rewrites += n;
                patchedStateNames.Add(stateName);
            }
        }

        // 2) One-time discovery if known list missed (log names so we can hardcode later).
        if (rewrites == 0 && sprint.FsmStates != null)
        {
            foreach (FsmState state in sprint.FsmStates)
            {
                if (state == null) continue;
                int n = RewriteHasDashTests(state);
                if (n > 0)
                {
                    rewrites += n;
                    patchedStateNames.Add(state.Name);
                }
            }
            if (rewrites > 0)
            {
                ItemChangerPlugin.Instance.Logger.LogInfo(
                    $"[GroundedSprint] discovery: add these state names to KnownHasDashGateStates: {string.Join(", ", patchedStateNames)}");
            }
        }

        if (rewrites == 0)
        {
            ItemChangerPlugin.Instance.Logger.LogWarning(
                "[GroundedSprint] no PlayerDataBoolTest(hasDash) found on sprintFSM — sprint gate not rewritten.");
            return;
        }

        _fsmPatched = true;
        ItemChangerPlugin.Instance.Logger.LogInfo(
            $"[GroundedSprint] sprintFSM hasDash gate rewritten ({rewrites} action(s) in states: {string.Join(", ", patchedStateNames)}).");
    }

    private static int RewriteHasDashTests(FsmState state)
    {
        int rewrites = 0;
        state.ReplaceActionsOfType<PlayerDataBoolTest>(orig =>
        {
            if (orig.boolName?.Value != nameof(PlayerData.hasDash))
                return orig;
            rewrites++;
            return new CustomCheckFsmStateAction(orig)
            {
                GetIsTrue = ShouldAllowSprint,
            };
        });
        return rewrites;
    }

    /// <summary>
    /// True when vanilla Swift Step is owned, or Grounded Sprint is owned and Hornet is on the ground.
    /// </summary>
    private static bool ShouldAllowSprint()
    {
        PlayerData? pd = PlayerData.instance;
        if (pd != null && pd.hasDash)
            return true;

        GroundedSprintModule? module = _activeInstance;
        if (module == null || !module.hasGroundedSprint)
            return false;

        HeroController? hc = HeroController.SilentInstance;
        return hc != null && hc.cState.onGround;
    }

    private static bool OnlyGroundedSprintKit()
    {
        GroundedSprintModule? module = _activeInstance;
        if (module == null || !module.hasGroundedSprint)
            return false;
        PlayerData? pd = PlayerData.instance;
        return pd == null || !pd.hasDash;
    }

    // ---- Air / shuttle-cock guards (issue #52: no air logic benefit) ----

    private static void HeroUpdatePostfix(HeroController __instance)
    {
        if (!OnlyGroundedSprintKit()) return;

        if (!__instance.cState.onGround)
        {
            if (__instance.cState.isSprinting || __instance.cState.isBackSprinting)
                __instance.sprintFSM?.SendEvent("CANCEL SPRINT");
            SprintBufferStepsField?.SetValue(__instance, 0);
            return;
        }

        if (__instance.cState.isSprinting || __instance.cState.isBackSprinting)
            __instance.PreventShuttlecock();
    }

    private static void LeftGroundPostfix(HeroController __instance)
    {
        if (!OnlyGroundedSprintKit()) return;
        SprintBufferStepsField?.SetValue(__instance, 0);
        if (__instance.cState.isSprinting || __instance.cState.isBackSprinting)
            __instance.sprintFSM?.SendEvent("CANCEL SPRINT");
    }

    private static void HeroJumpBoolPrefix(ref bool checkSprint)
    {
        if (!OnlyGroundedSprintKit()) return;
        checkSprint = false;
    }
}
