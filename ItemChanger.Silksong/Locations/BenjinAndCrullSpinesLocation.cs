using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Costs;
using ItemChanger.Enums;
using ItemChanger.Locations;
using ItemChanger.Placements;
using ItemChanger.Silksong.Extensions;
using ItemChanger.Silksong.Modules.YNBox;
using ItemChanger.Silksong.RawData;
using ItemChanger.Tags;
using PrepatcherPlugin;
using QuestPlaymakerActions;
using Silksong.FsmUtil;

namespace ItemChanger.Silksong.Locations;

public class BenjinAndCrullSpinesLocation : AutoLocation
{
    public override bool SupportsCost => true;

    protected override void DoLoad()
    {
        Using(new FsmEditGroup()
        {
            { new(SceneName!, "Dust Traders", "Dialogue"), HookDustTraders }
        });
    }

    protected override void DoUnload()
    {
    }

    private void HookDustTraders(PlayMakerFSM fsm)
    {
        // Reroute to Pins dialogue tree if items are yet to be obtained
        FsmState dialogTreeCheckState = fsm.MustGetState("State?");
        dialogTreeCheckState.RemoveFirstActionOfType<CheckQuestStateV2>();
        dialogTreeCheckState.InsertMethod(1, () =>
        {
            FullQuestBase quest = QuestManager.GetQuest(Quests.Doctor_Curse_Cure);
            if (!quest.IsAccepted)
                return;

            // always offer at least once, even if the placement is empty
            // reoffer if there are any items
            if (!PlayerDataAccess.DustTradersOfferedPins || !Placement!.AllObtained())
            {
                fsm.SendEvent("PINS");
            }
        });

        // Overwrite "has pins?" check
        FsmState checkPinsState = fsm.MustGetState("Pins State?");
        checkPinsState.RemoveFirstActionOfType<CollectableItemGetDataV3>();

        // Override yes/no box
        FsmState buyPinsState = fsm.MustGetState("Buy Pins?");
        buyPinsState.AddTransition("SKIP COST", "Give Pins");
        buyPinsState.Actions = [];
        buyPinsState.AddMethod(() =>
        {
            Cost? spinesCost = (Placement as ISingleCostPlacement)!.Cost ?? GetTag<DefaultCostTag>()?.Cost; // TODO: remove DCT check once IC.C updates
            if (spinesCost == null || spinesCost.Paid)
            {
                fsm.SendEvent("SKIP COST");
                return;
            }

            Placement!.AddVisitFlag(VisitState.Previewed);
            CustomYNEnableModule.Open(
                cost: spinesCost,
                text: Placement!.GetUIName(),
                yes: () => { fsm.SendEvent("TRUE"); },
                no: () => { fsm.SendEvent("FALSE"); });
        });

        // End dialogue early, otherwise dialogue shows over the top of big UI def
        FsmState givePinsState = fsm.MustGetState("Give Pins");
        givePinsState.AddAction(new EndDialogue()
        {
            ReturnControl = false,
            ReturnHUD = false,
            Target = new FsmOwnerDefault() { OwnerOption = OwnerDefaultOption.UseOwner },
            UseChildren = false
        });
        
        // Replace granting spines with obtaining the placement
        givePinsState.RemoveFirstActionOfType<SavedItemGet>();
        givePinsState.AddLambdaMethod(this.CreateGiveAllDelegate(fsm.transform));
    }
}