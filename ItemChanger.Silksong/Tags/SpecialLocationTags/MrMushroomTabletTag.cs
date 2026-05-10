using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Containers;
using ItemChanger.Enums;
using ItemChanger.Locations;
using ItemChanger.Silksong.Containers;
using ItemChanger.Silksong.Extensions;
using ItemChanger.Silksong.Modules.YNBox;
using ItemChanger.Tags;
using ItemChanger.Tags.Constraints;
using Newtonsoft.Json;
using Silksong.FsmUtil;
using UnityEngine;

namespace ItemChanger.Silksong.Tags.SpecialLocationTags;

/// <summary>
/// Patches the Mr Mushroom riddle tablet (Aqueduct_05 / Putrified Ducts) so its
/// FSM-driven inspect routes through ItemChanger instead of starting the
/// vanilla Passing of the Age wish. The tablet is not a BasicNPC so
/// TabletContainer's OnStartDialogue hook never catches it; we intercept the
/// "Tablet Control" FSM directly.
///
/// FSM dump from the bundle lists state names including: Pause, Init, Inspect,
/// Inspection, Begin Quest?, BeginQuest, Dialogue, Pre Enter Effect, Show
/// Prompt, FINISHED. Exact transition graph isn't fully known from static
/// analysis; the patcher logs all state names on first hook and applies edits
/// defensively (skip + warn if a state is absent).
/// </summary>
[LocationTag]
public class MrMushroomTabletTag : Tag
{
    [JsonIgnore] private Location? _location;

    protected override void DoLoad(TaggableObject parent)
    {
        _location = (parent as Location)!;
        Using(new FsmEditGroup()
        {
            { new FsmId(_location.SceneName!, "Mr Mushroom Tablet", "Tablet Control"), PatchTabletControl }
        });
    }

    protected override void DoUnload(TaggableObject parent)
    {
        _location = null;
    }

    private void PatchTabletControl(PlayMakerFSM fsm)
    {
        // Dump state names so we can see what's actually there during runtime
        // testing. Promote/remove this once the FSM is confirmed.
        if (SilksongHost.Instance.Logger is { } log)
        {
            var names = new System.Text.StringBuilder("[MrMushroomTabletTag] Tablet Control states: ");
            foreach (FsmState s in fsm.FsmStates) { names.Append(s.Name); names.Append(", "); }
            log.LogInfo(names.ToString());
        }

        // Bail on inspect if already obtained — short-circuits the prompt at
        // the earliest state we can find.
        TryInsertObtainedShortCircuit(fsm, "Pause");
        TryInsertObtainedShortCircuit(fsm, "Init");
        TryInsertObtainedShortCircuit(fsm, "Idle");

        // Find a state that runs dialogue / starts the quest and replace it
        // with an IC delivery hand-off. Candidate state names cover the
        // observed FSM string list.
        string[] candidates = ["Dialogue", "Inspection", "Begin Quest?", "BeginQuest", "Inspect"];
        FsmState? deliverState = null;
        foreach (string n in candidates)
        {
            FsmState? s = TryGetState(fsm, n);
            if (s == null) continue;
            // Prefer a state that actually runs vanilla dialogue / quest action.
            bool hasDialogue = false;
            foreach (FsmStateAction a in s.Actions)
            {
                if (a is CallMethodProper or SendEventByName)
                {
                    hasDialogue = true;
                    break;
                }
            }
            if (hasDialogue) { deliverState = s; break; }
            deliverState ??= s;
        }

        if (deliverState == null)
        {
            SilksongHost.Instance.Logger?.LogWarn(
                $"MrMushroomTabletTag: no candidate delivery state found in FSM 'Tablet Control'.");
            return;
        }

        // Strip vanilla dialogue + quest-start actions and route through IC.
        deliverState.RemoveActionsOfType<CallMethodProper>();
        deliverState.RemoveActionsOfType<SendEventByName>();
        deliverState.InsertMethod(0, () => Deliver(fsm));
    }

    private static FsmState? TryGetState(PlayMakerFSM fsm, string name)
    {
        foreach (FsmState s in fsm.FsmStates)
        {
            if (s.Name == name) return s;
        }
        return null;
    }

    private void TryInsertObtainedShortCircuit(PlayMakerFSM fsm, string stateName)
    {
        FsmState? s = TryGetState(fsm, stateName);
        if (s == null) return;
        s.InsertMethod(0, () =>
        {
            if (_location?.Placement?.AllObtained() == true)
            {
                fsm.SendEvent("CANCEL");
            }
        });
    }

    private void Deliver(PlayMakerFSM fsm)
    {
        if (_location?.Placement is null)
        {
            return;
        }

        if (_location.Placement.AllObtained())
        {
            return;
        }

        Transform t = fsm.gameObject.transform;
        ContainerInfo cInfo = ContainerInfo.FromPlacement(
            _location.Placement,
            t.gameObject.scene,
            ContainerNames.Tablet,
            _location.FlingType
        );

        SavedContainerItem item = ScriptableObject.CreateInstance<SavedContainerItem>();
        item.ContainerInfo = cInfo;
        item.ContainerTransform = t;
        item.SupportedMessageTypes = MessageType.Any;

        if (cInfo.CostInfo is null)
        {
            item.Get();
        }
        else
        {
            CustomYNEnableModule.Open(
                () => item.Get(),
                () => { },
                cInfo.CostInfo.Cost,
                cInfo.CostInfo.GetUIName()
            );
        }
    }
}
