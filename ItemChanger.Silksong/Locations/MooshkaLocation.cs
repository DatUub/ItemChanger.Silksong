using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Locations;
using Silksong.FsmUtil;

namespace ItemChanger.Silksong.Locations;

/// <summary>
/// AutoLocation for items granted by Mooshka (the Caravan Troupe Leader) when
/// flea-rescue thresholds are met. The same dialogue FSM lives on
/// "Caravan Troupe Leader * NPC" at each caravan stop; each subclass picks
/// the scene + reward state(s).
/// </summary>
public abstract class MooshkaLocation : AutoLocation
{
    /// <summary>NPC GameObject name in-scene, e.g. "Caravan Troupe Leader Bone NPC".</summary>
    public string NpcObjectName { get; set; } = "";

    /// <summary>FSM state(s) that grant the reward vanilla items. All vanilla item-give actions
    /// on these states are stripped and replaced with a GiveAll on the first one.</summary>
    public string[] RewardStates { get; set; } = [];

    protected override void DoLoad()
    {
        Using(new FsmEditGroup()
        {
            { new(SceneName!, NpcObjectName, "Dialogue"), ModifyMooshkaFsm },
        });
    }

    protected override void DoUnload() { }

    private void ModifyMooshkaFsm(PlayMakerFSM fsm)
    {
        bool gave = false;
        foreach (var stateName in RewardStates)
        {
            FsmState? state = fsm.GetState(stateName);
            if (state == null) continue;

            // Strip vanilla grant actions.
            state.RemoveActionsOfType<SavedItemGetV2>();
            state.RemoveActionsOfType<CreateUIMsgGetItem>();
            state.RemoveActionsOfType<SetPlayerDataVariable>();
            state.RemoveActionsOfType<SetPlayerDataBool>();
            state.RemoveActionsOfType<SetPlayerDataInt>();
            state.RemoveActionsOfType<SetFsmString>();

            if (!gave)
            {
                state.InsertMethod(0, () =>
                {
                    var obj = GameObject.Find(NpcObjectName);
                    Placement?.GiveAll(new()
                    {
                        FlingType = Enums.FlingType.Everywhere,
                        MessageType = Enums.MessageType.Any,
                        Transform = obj != null ? obj.transform : null,
                    });
                });
                gave = true;
            }
        }
    }
}

/// <summary>Mooshka at Marrow (Bone_10). Hosts Flea Brew on first arrival.</summary>
public class MooshkaBoneLocation : MooshkaLocation
{
    public MooshkaBoneLocation()
    {
        NpcObjectName = "Caravan Troupe Leader Bone NPC";
        RewardStates = ["Give Brew"];
    }
}

/// <summary>Mooshka at Greymoor (Greymoor_08_caravan). Hosts Flea Brew if reached here.</summary>
public class MooshkaGreymoorLocation : MooshkaLocation
{
    public MooshkaGreymoorLocation()
    {
        NpcObjectName = "Caravan Troupe Leader Greymoor NPC";
        RewardStates = ["Give Brew"];
    }
}

/// <summary>Mooshka at Blasted Steps (Coral_Judge_Arena). Awards Spool Fragment + can grant Brew.</summary>
public class MooshkaJudgeLocation : MooshkaLocation
{
    public MooshkaJudgeLocation()
    {
        NpcObjectName = "Caravan Troupe Leader Judge NPC";
        RewardStates = ["Award Spool Piece"];
    }
}

/// <summary>Mooshka at Fleatopia (Aqueduct_05_caravan). Awards Tool Pouch and Egg of Flealia.</summary>
/// <remarks>Each placement hooks its own award state; two placements may co-exist on this NPC.</remarks>
public class MooshkaFleatopiaLocation : MooshkaLocation
{
    public MooshkaFleatopiaLocation()
    {
        NpcObjectName = "Caravan Troupe Leader Fleatopia NPC";
    }
}
