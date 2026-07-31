using HarmonyLib;
using ItemChanger.Silksong.Extensions;
using System.Reflection;
using UnityEngine;

namespace ItemChanger.Silksong.Modules.CustomSkills;

/// <summary>
/// Novelty: Swift Step sprint while grounded, without air dash / air sprint / shuttle-cock.
///
/// flibber (#209): trick the game so vanilla sprint anim/cState run — not a GetRunSpeed hack.
///
/// Silksong path: CanDash (field hasDash) → HeroDash → sprintFSM DASHED → TRY SPRINT.
/// Sprint speed is FSM "Add Speed". CanDash is postfixed true only on ground for GS-only.
///
/// Air / shuttle-cock (playtest failures):
/// - Prefix OnShuttleCockJump to skip entirely for GS-only kit
/// - Prefix HeroJump(bool) forces checkSprint=false
/// - PreventShuttlecock every frame while GS-only (covers dashing→jump)
/// - Air: CANCEL SPRINT, clear cState sprint flags, zero buffer + Add Speed
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
    private static readonly FieldInfo? SprintSpeedAddFloatField =
        AccessTools.Field(typeof(HeroController), "sprintSpeedAddFloat");
    private static readonly FieldInfo? NoShuttlecockTimeField =
        AccessTools.Field(typeof(HeroController), "noShuttlecockTime");

    private Harmony? _harmony;

    public override IEnumerable<string> GettableSkillBools() =>
    [
        nameof(hasGroundedSprint),
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

    private bool ComputeEffectiveHasDash()
    {
        if (ReadRawHasDash()) return true;
        if (!hasGroundedSprint) return false;
        HeroController? hc = HeroController.SilentInstance;
        return hc != null && hc.cState.onGround;
    }

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

    protected override void DoLoad()
    {
        base.DoLoad();
        _activeInstance = this;

        Using(Md.InventoryItemConditional.Evaluate.Prefix(OverrideInventoryDisplayTest));

        _harmony = new Harmony("itemchanger.silksong.groundedsprint");

        Patch(typeof(HeroController), nameof(HeroController.CanDash),
            postfix: nameof(CanDashPostfix));

        // Private Update — per-frame air cancel + continuous shuttlecock block
        Patch(typeof(HeroController), "Update", postfix: nameof(HeroUpdatePostfix));
        Patch(typeof(HeroController), "FixedUpdate", postfix: nameof(HeroFixedUpdatePostfix));

        // LeftGround fills sprintBufferSteps when isSprinting/dashing — zero after.
        var leftGround = AccessTools.Method(typeof(HeroController), "LeftGround", [typeof(bool)]);
        if (leftGround != null)
        {
            _harmony.Patch(leftGround,
                postfix: new HarmonyMethod(typeof(GroundedSprintModule), nameof(LeftGroundPostfix)));
        }

        // HeroJump(bool): collapse checkSprint
        var heroJumpBool = AccessTools.Method(typeof(HeroController), "HeroJump", [typeof(bool)]);
        if (heroJumpBool != null)
        {
            _harmony.Patch(heroJumpBool,
                prefix: new HarmonyMethod(typeof(GroundedSprintModule), nameof(HeroJumpBoolPrefix)));
        }

        // Hard block: skip OnShuttleCockJump body entirely for GS-only
        var shuttle = AccessTools.Method(typeof(HeroController), "OnShuttleCockJump");
        if (shuttle != null)
        {
            _harmony.Patch(shuttle,
                prefix: new HarmonyMethod(typeof(GroundedSprintModule), nameof(OnShuttleCockJumpPrefix)));
        }
        else
        {
            ItemChangerPlugin.Instance.Logger.LogWarning(
                "[GroundedSprint] OnShuttleCockJump not found — shuttlecock may still fire.");
        }

        ItemChangerPlugin.Instance.Logger.LogInfo(
            "[GroundedSprint] loaded: grounded CanDash/GetBool + hard shuttlecock skip + air cancel.");
    }

    private void Patch(Type type, string name, string? prefix = null, string? postfix = null)
    {
        MethodInfo? m = AccessTools.Method(type, name);
        if (m == null)
        {
            ItemChangerPlugin.Instance.Logger.LogWarning($"[GroundedSprint] method not found: {type.Name}.{name}");
            return;
        }
        _harmony!.Patch(m,
            prefix: prefix != null ? new HarmonyMethod(typeof(GroundedSprintModule), prefix) : null,
            postfix: postfix != null ? new HarmonyMethod(typeof(GroundedSprintModule), postfix) : null);
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

    // ---- Harmony targets ----

    private static void CanDashPostfix(HeroController __instance, ref bool __result)
    {
        if (__result) return;
        if (!OnlyGroundedSprintKit()) return;
        if (!__instance.cState.onGround) return;
        if (__instance.cState.hazardDeath) return;
        __result = true;
    }

    /// <summary>Harmony prefix: return false to skip original OnShuttleCockJump.</summary>
    private static bool OnShuttleCockJumpPrefix()
    {
        if (!OnlyGroundedSprintKit()) return true; // run original
        return false; // skip — no shuttle-cock for GS-only
    }

    private static void HeroJumpBoolPrefix(HeroController __instance, ref bool checkSprint)
    {
        if (!OnlyGroundedSprintKit()) return;
        checkSprint = false;
        // Belt-and-suspenders: keep prevent window open and clear buffer before jump body.
        __instance.PreventShuttlecock();
        SprintBufferStepsField?.SetValue(__instance, 0);
        // Far-future prevent in case PreventShuttlecock window is too short during frame spikes
        NoShuttlecockTimeField?.SetValue(__instance, Time.timeAsDouble + 5.0);
    }

    private static void HeroUpdatePostfix(HeroController __instance) => TickGroundedSprintGuards(__instance);
    private static void HeroFixedUpdatePostfix(HeroController __instance) => TickGroundedSprintGuards(__instance);

    private static void TickGroundedSprintGuards(HeroController hc)
    {
        if (!OnlyGroundedSprintKit()) return;

        // Always keep shuttlecock blocked while GS-only (covers dashing frames before isSprinting).
        hc.PreventShuttlecock();
        NoShuttlecockTimeField?.SetValue(hc, Time.timeAsDouble + 1.0);

        if (!hc.cState.onGround)
        {
            CancelSprintHard(hc);
            return;
        }

        // Hold-dash on ground: poke TRY SPRINT if idle walk (FSM may need the nudge).
        if (InputHandler.Instance != null
            && InputHandler.Instance.inputActions.Dash.IsPressed
            && !hc.cState.dashing
            && !hc.cState.isSprinting
            && !hc.cState.isBackSprinting
            && hc.CanSprint())
        {
            hc.sprintFSM?.SendEvent("TRY SPRINT");
        }
    }

    private static void LeftGroundPostfix(HeroController __instance)
    {
        if (!OnlyGroundedSprintKit()) return;
        CancelSprintHard(__instance);
    }

    /// <summary>
    /// Force end of sprint state for GS-only: FSM event + cState + buffer + Add Speed float.
    /// </summary>
    private static void CancelSprintHard(HeroController hc)
    {
        SprintBufferStepsField?.SetValue(hc, 0);

        bool wasSprinting = hc.cState.isSprinting
            || hc.cState.isBackSprinting
            || hc.cState.isBackScuttling
            || (hc.sprintFSM != null && hc.sprintFSM.FsmVariables.GetFsmBool("Is Sprinting") is { } b && b.Value);

        if (wasSprinting || !hc.cState.onGround)
        {
            hc.sprintFSM?.SendEvent("CANCEL SPRINT");
            // Direct cState clear if FSM is slow a frame
            hc.cState.isSprinting = false;
            hc.cState.isBackSprinting = false;
        }

        // Zero sprint speed add so residual velocity doesn't linger mid-air
        if (SprintSpeedAddFloatField?.GetValue(hc) is HutongGames.PlayMaker.FsmFloat add)
            add.Value = 0f;
    }
}
