using HarmonyLib;
using ItemChanger.Silksong.Extensions;
using System.Reflection;
using UnityEngine;

namespace ItemChanger.Silksong.Modules.CustomSkills;

/// <summary>
/// Novelty: Swift Step sprint while grounded only — no air sprint, no air dash, no shuttle-cock.
///
/// flibber (#209): trick the game so vanilla sprint anim/cState run.
///
/// Playtest fixes:
/// - Ledge fall kept sprinting: CANCEL SPRINT alone is not enough; clamp air horizontal
///   speed to walk and zero FSM Add Speed every physics tick while airborne.
/// - Wall clip on turn+jump: do NOT spoof CanDash (that starts a real ground dash into
///   walls). Enter sprint only via TRY SPRINT + GetBool("hasDash") while grounded.
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
    private static readonly FieldInfo? SyncBufferStepsField =
        AccessTools.Field(typeof(HeroController), "syncBufferSteps");

    private Harmony? _harmony;
    private static bool _wasOnGround = true;

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
        // Strict ground only — false the instant we leave ground so FSM gates flip.
        HeroController? hc = HeroController.SilentInstance;
        return hc != null && IsEffectivelyGrounded(hc);
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

    /// <summary>
    /// Grounded for GS purposes: onGround flag OR still touching floor this frame.
    /// Leaving ledge often has a frame where velocity is high but flag lags — treat
    /// !CheckTouchingGround as air for cancel/clamp.
    /// </summary>
    private static bool IsEffectivelyGrounded(HeroController hc)
    {
        if (hc.cState.onGround) return true;
        // Don't treat wall-slide as ground sprint surface
        if (hc.cState.wallSliding || hc.cState.wallClinging || hc.cState.wallScrambling)
            return false;
        try
        {
            return hc.CheckTouchingGround();
        }
        catch
        {
            return false;
        }
    }

    protected override void DoLoad()
    {
        base.DoLoad();
        _activeInstance = this;
        _wasOnGround = true;

        Using(Md.InventoryItemConditional.Evaluate.Prefix(OverrideInventoryDisplayTest));

        _harmony = new Harmony("itemchanger.silksong.groundedsprint");

        // Intentionally NOT spoofing CanDash — that starts a real ground dash and caused
        // wall clips on turn+jump. Sprint entry is TRY SPRINT + GetBool hasDash only.

        Patch(typeof(HeroController), "Update", postfix: nameof(HeroUpdatePostfix));
        Patch(typeof(HeroController), "FixedUpdate", postfix: nameof(HeroFixedUpdatePostfix));

        var leftGround = AccessTools.Method(typeof(HeroController), "LeftGround", [typeof(bool)]);
        if (leftGround != null)
        {
            _harmony.Patch(leftGround,
                prefix: new HarmonyMethod(typeof(GroundedSprintModule), nameof(LeftGroundPrefix)),
                postfix: new HarmonyMethod(typeof(GroundedSprintModule), nameof(LeftGroundPostfix)));
        }

        var heroJumpBool = AccessTools.Method(typeof(HeroController), "HeroJump", [typeof(bool)]);
        if (heroJumpBool != null)
        {
            _harmony.Patch(heroJumpBool,
                prefix: new HarmonyMethod(typeof(GroundedSprintModule), nameof(HeroJumpBoolPrefix)));
        }

        var shuttle = AccessTools.Method(typeof(HeroController), "OnShuttleCockJump");
        if (shuttle != null)
        {
            _harmony.Patch(shuttle,
                prefix: new HarmonyMethod(typeof(GroundedSprintModule), nameof(OnShuttleCockJumpPrefix)));
        }

        ItemChangerPlugin.Instance.Logger.LogInfo(
            "[GroundedSprint] loaded: GetBool grounded hasDash, TRY SPRINT only (no CanDash), hard air clamp.");
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

    // ---- Harmony ----

    private static bool OnShuttleCockJumpPrefix() => !OnlyGroundedSprintKit();

    private static void HeroJumpBoolPrefix(HeroController __instance, ref bool checkSprint)
    {
        if (!OnlyGroundedSprintKit()) return;
        checkSprint = false;
        __instance.PreventShuttlecock();
        SprintBufferStepsField?.SetValue(__instance, 0);
        SyncBufferStepsField?.SetValue(__instance, false);
        NoShuttlecockTimeField?.SetValue(__instance, Time.timeAsDouble + 5.0);
        CancelSprintHard(__instance, clampAirSpeed: false);
    }

    /// <summary>Before LeftGround fills sprintBufferSteps from isSprinting/dashing.</summary>
    private static void LeftGroundPrefix(HeroController __instance)
    {
        if (!OnlyGroundedSprintKit()) return;
        // Clear sprint flags BEFORE vanilla buffer fill (uses isSprinting || dashing).
        __instance.cState.isSprinting = false;
        __instance.cState.isBackSprinting = false;
        SprintBufferStepsField?.SetValue(__instance, 0);
        SyncBufferStepsField?.SetValue(__instance, false);
    }

    private static void LeftGroundPostfix(HeroController __instance)
    {
        if (!OnlyGroundedSprintKit()) return;
        CancelSprintHard(__instance, clampAirSpeed: true);
    }

    private static void HeroUpdatePostfix(HeroController __instance) => TickGuards(__instance, physics: false);
    private static void HeroFixedUpdatePostfix(HeroController __instance) => TickGuards(__instance, physics: true);

    private static void TickGuards(HeroController hc, bool physics)
    {
        if (!OnlyGroundedSprintKit())
        {
            _wasOnGround = hc.cState.onGround;
            return;
        }

        hc.PreventShuttlecock();
        NoShuttlecockTimeField?.SetValue(hc, Time.timeAsDouble + 1.0);

        bool grounded = IsEffectivelyGrounded(hc);

        // Edge: just left ground (ledge or jump) — cancel immediately.
        if (_wasOnGround && !grounded)
            CancelSprintHard(hc, clampAirSpeed: true);

        _wasOnGround = grounded;

        if (!grounded)
        {
            CancelSprintHard(hc, clampAirSpeed: true);
            if (physics)
                ClampAirHorizontalSpeed(hc);
            return;
        }

        // Ground: hold dash → enter vanilla sprint path without enabling CanDash/HeroDash.
        if (InputHandler.Instance != null
            && InputHandler.Instance.inputActions.Dash.IsPressed
            && !hc.cState.dashing
            && !hc.cState.hazardDeath
            && hc.CanSprint())
        {
            hc.sprintFSM?.SendEvent("TRY SPRINT");
        }
    }

    private static void CancelSprintHard(HeroController hc, bool clampAirSpeed)
    {
        SprintBufferStepsField?.SetValue(hc, 0);
        SyncBufferStepsField?.SetValue(hc, false);

        hc.sprintFSM?.SendEvent("CANCEL SPRINT");
        // Extra cancel events some sprint substates listen for
        hc.sprintFSM?.SendEvent("HARD LANDING");

        hc.cState.isSprinting = false;
        hc.cState.isBackSprinting = false;

        if (hc.sprintFSM != null)
        {
            var isSprint = hc.sprintFSM.FsmVariables.GetFsmBool("Is Sprinting");
            if (isSprint != null) isSprint.Value = false;
        }

        if (SprintSpeedAddFloatField?.GetValue(hc) is HutongGames.PlayMaker.FsmFloat add)
            add.Value = 0f;

        if (clampAirSpeed && !IsEffectivelyGrounded(hc))
            ClampAirHorizontalSpeed(hc);
    }

    /// <summary>
    /// Cap midair horizontal speed to walk so "still sprinting off a ledge" has no mobility gain.
    /// </summary>
    private static void ClampAirHorizontalSpeed(HeroController hc)
    {
        Rigidbody2D rb = hc.rb2d;
        if (rb == null) return;

        float max = Mathf.Abs(hc.GetWalkSpeed());
        if (max < 0.01f) max = 6f; // fallback if walk speed unreadable

        Vector2 v = rb.linearVelocity;
        if (Mathf.Abs(v.x) > max)
        {
            v.x = Mathf.Sign(v.x) * max;
            rb.linearVelocity = v;
        }
    }
}
