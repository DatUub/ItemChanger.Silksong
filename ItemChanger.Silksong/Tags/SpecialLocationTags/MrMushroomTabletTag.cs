using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using QuestPlaymakerActions;
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
/// "Inspection" FSM directly.
///
/// Bundle dump of aqueduct_05.bundle (GO "Mr Mushroom Tablet", FSM "Inspection"):
///   Idle -> Prompt Up -> Weaver Dialogue (RunDialogue) -> Prompt Down ->
///   Hornet Dialogue (RunDialogue) -> Begin Quest? (QuestYesNoV2) ->
///   {YES: Dialogue End Yes (BeginQuestV2), NO: Dialogue End No}.
/// IC delivery slots into Dialogue End Yes (the explicit YES-accept branch),
/// stripping BeginQuestV2 so the Passing of the Age wish doesn't start.
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
            { new FsmId(_location.SceneName!, "Mr Mushroom Tablet", "Inspection"), PatchInspection }
        });
    }

    protected override void DoUnload(TaggableObject parent)
    {
        _location = null;
    }

    private void PatchInspection(PlayMakerFSM fsm)
    {
        TryInsertObtainedShortCircuit(fsm, "Idle");

        FsmState? deliverState = TryGetState(fsm, "Dialogue End Yes");
        if (deliverState == null)
        {
            SilksongHost.Instance.Logger?.LogWarn(
                "MrMushroomTabletTag: 'Dialogue End Yes' state missing from FSM 'Inspection'.");
            return;
        }

        deliverState.RemoveActionsOfType<BeginQuestV2>();
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
        // CANCEL is the Idle->Inactive transition in the Inspection FSM,
        // skipping the dialogue + YES/NO prompt entirely.
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
