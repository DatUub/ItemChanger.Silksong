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
/// - Ledge fall: soft cancel + clamp air horizontal speed to walk.
/// - Wall clip: never allow real HeroDash for GS-only.
/// - Ground dash still worked: Prepatcher makes playerData.hasDash use GetBool, so
///   spoofing hasDash made CanDash true. Force CanDash false and block HeroDash*.
/// - Jump-from-sprint / downslash: never send HARD LANDING (kills jump Y and interrupts
///   air attacks). Only CANCEL SPRINT + flag clear, and only on leave-ground / jump —
///   not every airborne frame.
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

        // GetBool("hasDash") is true while grounded so the sprint FSM can run, but
        // Prepatcher routes playerData.hasDash through GetBool — so CanDash would also
        // become true. Force CanDash false and block HeroDash* for GS-only (sprint only).
        Patch(typeof(HeroController), nameof(HeroController.CanDash), postfix: nameof(CanDashPostfix));
        var heroDashPressed = AccessTools.Method(typeof(HeroController), "HeroDashPressed");
        if (heroDashPressed != null)
        {
            _harmony.Patch(heroDashPressed,
                prefix: new HarmonyMethod(typeof(GroundedSprintModule), nameof(HeroDashPressedPrefix)));
        }
        var heroDash = AccessTools.Method(typeof(HeroController), "HeroDash", [typeof(bool)]);
        if (heroDash != null)
        {
            _harmony.Patch(heroDash,
                prefix: new HarmonyMethod(typeof(GroundedSprintModule), nameof(HeroDashPrefix)));
        }

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
            "[GroundedSprint] loaded: GetBool hasDash for FSM, CanDash/HeroDash blocked, TRY SPRINT + air clamp.");
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

    /// <summary>
    /// Prepatcher makes hasDash property use GetBool — force dash ability off for GS-only.
    /// Sprint is entered via TRY SPRINT, not HeroDash.
    /// </summary>
    private static void CanDashPostfix(HeroController __instance, ref bool __result)
    {
        if (!OnlyGroundedSprintKit()) return;
        __result = false;
    }

    /// <summary>Skip HeroDashPressed entirely for GS-only (redirect dash button to sprint).</summary>
    private static bool HeroDashPressedPrefix(HeroController __instance)
    {
        if (!OnlyGroundedSprintKit()) return true;
        // Convert dash press into sprint attempt on ground only.
        if (__instance.cState.onGround && __instance.CanSprint())
            __instance.sprintFSM?.SendEvent("TRY SPRINT");
        return false; // skip original dash
    }

    private static bool HeroDashPrefix(HeroController __instance, bool startAlreadyDashing)
    {
        if (!OnlyGroundedSprintKit()) return true;
        // Never start a real dash with GS-only.
        if (__instance.cState.onGround && __instance.CanSprint())
            __instance.sprintFSM?.SendEvent("TRY SPRINT");
        return false;
    }

    private static bool OnShuttleCockJumpPrefix() => !OnlyGroundedSprintKit();

    private static void HeroJumpBoolPrefix(HeroController __instance, ref bool checkSprint)
    {
        if (!OnlyGroundedSprintKit()) return;
        // Disable shuttlecock path without wrecking jump velocity.
        checkSprint = false;
        __instance.PreventShuttlecock();
        SprintBufferStepsField?.SetValue(__instance, 0);
        SyncBufferStepsField?.SetValue(__instance, false);
        NoShuttlecockTimeField?.SetValue(__instance, Time.timeAsDouble + 5.0);
        // Soft cancel only — HARD LANDING / air clamp here was killing jump height.
        SoftCancelSprint(__instance, sendFsmEvent: true, clampAirSpeed: false);
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
        SoftCancelSprint(__instance, sendFsmEvent: true, clampAirSpeed: true);
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

        // Edge: just left ground (ledge or jump) — one soft cancel, not HARD LANDING.
        if (_wasOnGround && !grounded)
            SoftCancelSprint(hc, sendFsmEvent: true, clampAirSpeed: true);

        _wasOnGround = grounded;

        if (!grounded)
        {
            // Maintain no-sprint midair without re-firing FSM events every frame
            // (HARD LANDING spam broke jump height + downslash).
            if (hc.cState.isSprinting || hc.cState.isBackSprinting)
                SoftCancelSprint(hc, sendFsmEvent: true, clampAirSpeed: true);
            else
                ClearSprintSpeedAdd(hc);

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

    /// <summary>
    /// End sprint cleanly. Never send HARD LANDING — that event aborts jump impulse
    /// and interrupts aerial attacks (downslash).
    /// </summary>
    private static void SoftCancelSprint(HeroController hc, bool sendFsmEvent, bool clampAirSpeed)
    {
        SprintBufferStepsField?.SetValue(hc, 0);
        SyncBufferStepsField?.SetValue(hc, false);

        if (sendFsmEvent)
            hc.sprintFSM?.SendEvent("CANCEL SPRINT");

        hc.cState.isSprinting = false;
        hc.cState.isBackSprinting = false;

        if (hc.sprintFSM != null)
        {
            var isSprint = hc.sprintFSM.FsmVariables.GetFsmBool("Is Sprinting");
            if (isSprint != null) isSprint.Value = false;
        }

        ClearSprintSpeedAdd(hc);

        if (clampAirSpeed && !IsEffectivelyGrounded(hc))
            ClampAirHorizontalSpeed(hc);
    }

    private static void ClearSprintSpeedAdd(HeroController hc)
    {
        if (SprintSpeedAddFloatField?.GetValue(hc) is HutongGames.PlayMaker.FsmFloat add)
            add.Value = 0f;
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
