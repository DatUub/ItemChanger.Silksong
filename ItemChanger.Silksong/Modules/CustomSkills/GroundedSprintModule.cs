using HarmonyLib;
using ItemChanger.Silksong.Extensions;
using ItemChanger.Silksong.RawData;
using System.Reflection;

namespace ItemChanger.Silksong.Modules.CustomSkills;

/// <summary>
/// Novelty skill: sprint at Swift Step speed while grounded, without granting air sprint or dash.
/// Implements flibber's "trick the game" approach via <see cref="CustomSkillPlayerDataModule"/>:
/// <c>PlayerData.GetBool("hasDash")</c> returns true only when grounded + owned, so the vanilla
/// sprint FSM (animations, cState, Sprintmaster/Quickening tiers) runs unchanged.
/// Direct field reads of <c>playerData.hasDash</c> (e.g. <c>CanDash</c>) stay false until real Swift Step.
/// </summary>
public class GroundedSprintModule : CustomSkillModule
{
#pragma warning disable IDE1006 // Naming Styles — match PlayerData / skill bool convention
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
        // Intercept GetBool("hasDash") for sprint FSM / PlayMaker tests only.
        // Conflicts with SplitSwiftStep if both register hasDash — log error, last-loaded wins.
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
    /// True if the player owns real Swift Step (field), or owns Grounded Sprint and is on the ground.
    /// </summary>
    private bool ComputeEffectiveHasDash()
    {
        if (ReadRawHasDash()) return true;
        if (!hasGroundedSprint) return false;
        HeroController? hc = HeroController.SilentInstance;
        return hc != null && hc.cState.onGround;
    }

    /// <summary>Real Swift Step ownership via field — must not go through GetBool (recursion).</summary>
    private static bool ReadRawHasDash()
    {
        PlayerData? pd = PlayerData.instance;
        if (pd == null) return false;
        if (HasDashField != null)
            return (bool)HasDashField.GetValue(pd)!;
        // Fallback: may recurse if hasDash is a GetBool-backed property; prefer field.
        return false;
    }

    private static bool OnlyGroundedSprintKit()
    {
        GroundedSprintModule? module = _activeInstance;
        if (module == null || !module.hasGroundedSprint) return false;
        return !ReadRawHasDash();
    }

    protected override void DoLoad()
    {
        base.DoLoad();
        _activeInstance = this;

        // Inventory "has Swift Step?" display should treat Grounded Sprint as owned sprint kit.
        Using(Md.InventoryItemConditional.Evaluate.Prefix(OverrideInventoryDisplayTest));

        // Per-frame air cancel, shuttlecock block, coyote-buffer clear.
        // HeroController.Update may not have an Md hook; use Harmony.
        _harmony = new Harmony("itemchanger.silksong.groundedsprint");
        MethodInfo? update = AccessTools.Method(typeof(HeroController), "Update");
        if (update != null)
        {
            _harmony.Patch(update,
                postfix: new HarmonyMethod(typeof(GroundedSprintModule), nameof(HeroUpdatePostfix)));
        }
        else
        {
            ItemChangerPlugin.Instance.Logger.LogWarning(
                "[GroundedSprint] HeroController.Update not found; air-cancel / shuttlecock guards inactive.");
        }

        // LeftGround refills sprintBufferSteps when leaving ground while sprinting (coyote sprint-jump).
        MethodInfo? leftGround = AccessTools.Method(typeof(HeroController), "LeftGround", [typeof(bool)]);
        if (leftGround != null)
        {
            _harmony.Patch(leftGround,
                postfix: new HarmonyMethod(typeof(GroundedSprintModule), nameof(LeftGroundPostfix)));
        }
        else
        {
            // Some builds may only have BecomeAirborne; still zero buffer in Update.
            MethodInfo? becomeAirborne = AccessTools.Method(typeof(HeroController), "BecomeAirborne");
            if (becomeAirborne != null)
            {
                _harmony.Patch(becomeAirborne,
                    postfix: new HarmonyMethod(typeof(GroundedSprintModule), nameof(BecomeAirbornePostfix)));
            }
        }

        // HeroJump(bool checkSprint) — bool overload is what runs (no-arg is often inlined).
        MethodInfo? heroJumpBool = AccessTools.Method(
            typeof(HeroController), "HeroJump", [typeof(bool)]);
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
        if (_activeInstance == this) _activeInstance = null;
        base.DoUnload();
    }

    private void OverrideInventoryDisplayTest(InventoryItemConditional self)
    {
        // Only remap when Grounded Sprint is the sole sprint kit so we don't
        // disturb vanilla / SplitSwiftStep inventory for real hasDash owners.
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

    // ---- Harmony targets ----

    private static void HeroUpdatePostfix(HeroController __instance)
    {
        if (!OnlyGroundedSprintKit()) return;

        if (!__instance.cState.onGround)
        {
            // Leave ground while "sprinting" under grounded kit → cancel vanilla sprint FSM.
            if (__instance.cState.isSprinting || __instance.cState.isBackSprinting)
            {
                __instance.sprintFSM?.SendEvent("CANCEL SPRINT");
            }
            SprintBufferStepsField?.SetValue(__instance, 0);
            return;
        }

        // Grounded and actively sprinting: block shuttle-cock sprint-jump.
        if (__instance.cState.isSprinting || __instance.cState.isBackSprinting)
        {
            __instance.PreventShuttlecock();
        }
    }

    private static void LeftGroundPostfix(HeroController __instance)
    {
        if (!OnlyGroundedSprintKit()) return;
        SprintBufferStepsField?.SetValue(__instance, 0);
        if (__instance.cState.isSprinting || __instance.cState.isBackSprinting)
        {
            __instance.sprintFSM?.SendEvent("CANCEL SPRINT");
        }
    }

    private static void BecomeAirbornePostfix(HeroController __instance)
    {
        // Fallback if LeftGround is missing.
        LeftGroundPostfix(__instance);
    }

    private static void HeroJumpBoolPrefix(ref bool checkSprint)
    {
        if (!OnlyGroundedSprintKit()) return;
        // Collapse sprint-jump gate: if (checkSprint && (buffer || dashing || isSprinting) && time)
        checkSprint = false;
    }
}
