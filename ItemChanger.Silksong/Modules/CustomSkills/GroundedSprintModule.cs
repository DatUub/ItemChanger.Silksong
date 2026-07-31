using HarmonyLib;
using ItemChanger.Silksong.Extensions;
using System.Reflection;

namespace ItemChanger.Silksong.Modules.CustomSkills;

/// <summary>
/// Novelty: Swift Step sprint while grounded, without air dash / air sprint for logic.
///
/// flibber (#209): trick the game into thinking the player has dash at the right times
/// so vanilla animations and cState work — not a GetRunSpeed speed hack.
///
/// Silksong path (decomp): Dash.WasPressed → CanDash() [field hasDash] → HeroDash →
/// sprintFSM "DASHED" → FinishedDashing → "TRY SPRINT". Sprint speed is FSM "Add Speed",
/// not GetRunSpeed. There is no PlayerDataBoolTest(hasDash) on sprintFSM.
///
/// We:
/// 1) Spoof GetBool("hasDash") while grounded + owned (CustomSkillPlayerDataModule)
/// 2) Postfix CanDash so ground dash→sprint can start (field hasDash stays false for air)
/// 3) Cancel sprint / clear buffer / PreventShuttlecock in air
/// </summary>
public class GroundedSprintModule : CustomSkillModule
{
#pragma warning disable IDE1006
    public bool hasGroundedSprint { get; set; }
#pragma warning restore IDE1006

    private static GroundedSprintModule? _activeInstance;
    private static readonly FieldInfo? SprintBufferStepsField =
        AccessTools.Field(typeof(HeroController), "sprintBufferSteps");
    private static readonly FieldInfo? HasDashField =
        AccessTools.Field(typeof(PlayerData), "hasDash")
        ?? AccessTools.DeclaredField(typeof(PlayerData), "hasDash");

    private Harmony? _harmony;

    public override IEnumerable<string> GettableSkillBools() =>
    [
        nameof(hasGroundedSprint),
        // GetPlayerDataBool / PlayMaker path (and inventory tests via GetBool).
        // Conflicts with SplitSwiftStep if both register hasDash — rare; log error on load.
        nameof(PlayerData.hasDash),
    ];

    public override bool GetBool(string boolName) => boolName switch
    {
        nameof(hasGroundedSprint) => hasGroundedSprint,
        nameof(PlayerData.hasDash) => ComputeEffectiveHasDash(),
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

    /// <summary>
    /// True if real Swift Step (field) or Grounded Sprint while on the ground.
    /// </summary>
    private bool ComputeEffectiveHasDash()
    {
        if (ReadRawHasDash()) return true;
        if (!hasGroundedSprint) return false;
        HeroController? hc = HeroController.SilentInstance;
        return hc != null && hc.cState.onGround;
    }

    /// <summary>True field ownership — never call GetBool (recursion).</summary>
    private static bool ReadRawHasDash()
    {
        PlayerData? pd = PlayerData.instance;
        if (pd == null) return false;
        if (HasDashField != null)
            return (bool)HasDashField.GetValue(pd)!;
        return false;
    }

    private static bool OnlyGroundedSprintKit()
    {
        GroundedSprintModule? module = _activeInstance;
        if (module == null || !module.hasGroundedSprint) return false;
        return !ReadRawHasDash();
    }

    private static bool GroundedSprintActiveOnGround()
    {
        if (!OnlyGroundedSprintKit()) return false;
        HeroController? hc = HeroController.SilentInstance;
        return hc != null && hc.cState.onGround;
    }

    protected override void DoLoad()
    {
        base.DoLoad();
        _activeInstance = this;

        // Inventory: show sprint-related entries when GS owned (stable, not ground-gated).
        Using(Md.InventoryItemConditional.Evaluate.Prefix(OverrideInventoryDisplayTest));

        _harmony = new Harmony("itemchanger.silksong.groundedsprint");

        // CanDash uses field hasDash — must postfix so ground dash→sprint can start.
        MethodInfo? canDash = AccessTools.Method(typeof(HeroController), nameof(HeroController.CanDash));
        if (canDash != null)
        {
            _harmony.Patch(canDash,
                postfix: new HarmonyMethod(typeof(GroundedSprintModule), nameof(CanDashPostfix)));
        }
        else
        {
            ItemChangerPlugin.Instance.Logger.LogWarning("[GroundedSprint] CanDash not found.");
        }

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

        MethodInfo? heroJumpBool = AccessTools.Method(typeof(HeroController), "HeroJump", [typeof(bool)]);
        if (heroJumpBool != null)
        {
            _harmony.Patch(heroJumpBool,
                prefix: new HarmonyMethod(typeof(GroundedSprintModule), nameof(HeroJumpBoolPrefix)));
        }

        ItemChangerPlugin.Instance.Logger.LogInfo(
            "[GroundedSprint] loaded: CanDash/GetBool grounded spoof + air cancel guards.");
    }

    protected override void DoUnload()
    {
        _harmony?.UnpatchSelf();
        _harmony = null;
        if (_activeInstance == this) _activeInstance = null;
        base.DoUnload();
    }

    private void OverrideInventoryDisplayTest(InventoryItemConditional self)
    {
        if (!hasGroundedSprint || ReadRawHasDash()) return;
        if (self.Test.IsSingleTest(out PlayerDataTest.Test t) && t.FieldName == nameof(PlayerData.hasDash))
        {
            self.Test.Modify(test =>
            {
                test.FieldName = nameof(hasGroundedSprint);
                return test;
            });
        }
    }

    // ---- Harmony ----

    /// <summary>
    /// Field hasDash is false for GS-only. Allow CanDash on ground so HeroDash → DASHED → sprint runs.
    /// Air stays false → no air dash.
    /// </summary>
    private static void CanDashPostfix(HeroController __instance, ref bool __result)
    {
        if (__result) return;
        if (!OnlyGroundedSprintKit()) return;
        if (!__instance.cState.onGround) return;
        // Match other CanDash gates loosely: no hazard death, allow input path.
        if (__instance.cState.hazardDeath) return;
        __result = true;
    }

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

        // Also poke TRY SPRINT while holding dash on ground if not already sprinting/dashing,
        // so hold-to-sprint works even when a full dash burst is interrupted.
        if (InputHandler.Instance != null
            && InputHandler.Instance.inputActions.Dash.IsPressed
            && !__instance.cState.dashing
            && !__instance.cState.isSprinting
            && __instance.CanSprint())
        {
            __instance.sprintFSM?.SendEvent("TRY SPRINT");
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
