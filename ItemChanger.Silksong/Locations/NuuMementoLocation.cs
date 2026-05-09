using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Locations;
using ItemChanger.Serialization;
using ItemChanger.Silksong.Extensions;
using ItemChanger.Silksong.Modules.BossKillsCounter;
using ItemChanger.Silksong.RawData;
using ItemChanger.Silksong.Serialization;
using Silksong.FsmUtil;

namespace ItemChanger.Silksong.Locations;

public class NuuMementoLocation : AutoLocation
{
    /// <summary>
    /// Number of boss kills required for obtaining this location
    /// </summary>
    public required int RequiredBossKills { get; init; }

    protected override void DoLoad()
    {
        Using(new FsmEditGroup()
        {
            { new(UnsafeSceneName, "Nuu", "Dialogue"), HookGiveMemento }
        });
    }

    protected override void DoUnload()
    {
    }

    private void HookGiveMemento(PlayMakerFSM fsm)
    {
        FsmState convoChoice = fsm.MustGetState("Convo Choice");
        convoChoice.InsertMethod(0, () =>
        {
            // allow obtaining memento after tool pouch without reloading the room.
            if (ReadyToCollect() && !Placement!.AllObtained()) fsm.GetBoolVariable("Spoken").Value = false;
        });

        // Replace journal completion check
        FsmState completionEvaluateState = fsm.MustGetState("Completion Evaluate");
        completionEvaluateState.Actions = [];
        completionEvaluateState.AddMethod(() =>
        {
            if (ReadyToCollect()) fsm.SendEvent("COMPLETED ALL");
        });

        // Give the placement
        FsmState giveMementoState = fsm.MustGetState("Give Memento");
        giveMementoState.RemoveFirstActionOfType<CollectableItemCollect>();
        giveMementoState.InsertLambdaMethod(3, GiveAll);
        
        // Prevent needing a room reload between obtaining/completing quest and obtaining memento
        fsm.MustGetState("Talk Journal Give").RemoveFirstActionOfType<SetBoolValue>();
        fsm.MustGetState("Talk Journal Given").RemoveFirstActionOfType<SetBoolValue>();
        fsm.MustGetState("Quest Completed").RemoveFirstActionOfType<SetBoolValue>();

        FsmState fullCompletion1 = fsm.MustGetState("Full Completion 1");
        fullCompletion1.RemoveActionsOfType<RunDialogue>();
        fullCompletion1.AddDynamicDialogueActions(GetProgressText);
    }

    private string GetProgressText()
    {
        return CompositeString.Create(ItemChangerLanguageStrings.FMT_HUNTER_FAN_COMPLETION_RESULT(), new Dictionary<string, IValueProvider<object>>()
        {
            { "COUNTER", ActiveProfile!.Modules.GetOrAdd<BossKillsCounterModule>().GetKillCount().ToValueProvider().Embox() },
            { "TARGET", RequiredBossKills.ToValueProvider().Embox() },
        }).Value;
    }

    private bool ReadyToCollect() => ActiveProfile!.Modules.GetOrAdd<BossKillsCounterModule>().GetKillCount() >= RequiredBossKills;
}