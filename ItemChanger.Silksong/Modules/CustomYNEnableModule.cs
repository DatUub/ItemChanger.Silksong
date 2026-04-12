using HarmonyLib;
using ItemChanger.Costs;
using ItemChanger.Modules;
using ItemChanger.Silksong.Components;
using ItemChanger.Silksong.Costs;
using MonoDetour.DetourTypes;

namespace ItemChanger.Silksong.Modules;

/// <summary>
/// Module that allows for InteractEvents components to send more customizable YN boxes,
/// provided that the gameObject has a <see cref="CustomYNBoxInfo"/> component.
/// </summary>
public class CustomYNEnableModule : Module
{
    protected override void DoLoad()
    {
        // When an interact events is interacted with and there's a CustomYNBoxInfo component,
        // we need to modify the call to DialogueYesNoBox.Open
        Using(Md.InteractEvents.ShowYesNo.ControlFlowPrefix(OverrideInteractEventsShow));
        // Allow showing cost text, rather than sprite + image
        Using(Md.SavedItemDisplay.Setup.ControlFlowPrefix(OverrideSavedItemDisplay));
    }

    protected override void DoUnload() { }

    private ReturnFlow OverrideSavedItemDisplay(SavedItemDisplay self, ref SavedItem item, ref int amount)
    {
        if (item is not ItemChangerCostOwner owner)
        {
            return ReturnFlow.None;
        }

        if (owner.Cost is IDisplayCost { DisplayEnabled: true })
        {
            return ReturnFlow.None;
        }

        self.icon.sprite = null;
        self.amountText.text = owner.Cost.GetCostText();

        return ReturnFlow.SkipOriginal;
    }
    
    private ReturnFlow OverrideInteractEventsShow(InteractEvents self)
    {
        CustomYNBoxInfo info = self.gameObject.GetComponent<CustomYNBoxInfo>();
        if (info == null)
        {
            return ReturnFlow.None;
        }

        // Event backing field needs reflection to access
        Action? origInteracted = AccessTools.Field(typeof(InteractEvents), nameof(InteractEvents.Interacted)).GetValue(self) as Action;

        DoOpen(info.Cost.Pay + origInteracted, self.EndInteraction, info.Cost, info.TextGetter());
        return ReturnFlow.SkipOriginal;
    }

    public void DoOpen(Action? yes, Action? no, Cost cost, string text)
    {
        if (cost is ICurrencyCost currencyCost)
        {
            DialogueYesNoBox.Open(yes, no, true, text, currencyCost.CurrencyType, currencyCost.Amount, consumeCurrency: false);
            return;
        }

        // Check if the costs are all displayable

        List<Cost> costs = (new MultiCost(cost)).ToList();
        List<Cost> displayableCosts = [];
        List<Cost> nonDisplayableCosts = [];
        foreach (Cost inner in new MultiCost(cost))
        {
            if (inner is IDisplayCost { DisplayEnabled: true })
            {
                displayableCosts.Add(inner);
            }
            else
            {
                nonDisplayableCosts.Add(inner);
            }
        }

        if (nonDisplayableCosts.Count > 0)
        {
            ItemChangerCostOwner costOwner = ItemChangerCostOwner.FromCost(cost);
            DialogueYesNoBox.Open(yes, no, true, text, [costOwner], [1], displayHudPopup: true, consumeCurrency: false, null);
            return;
        }

        List<ItemChangerCostOwner> displays = displayableCosts.Select(x => ItemChangerCostOwner.FromCost(x)).ToList();
        List<int> amounts = displayableCosts.Cast<IDisplayCost>().Select(x => x.Amount).ToList();
        DialogueYesNoBox.Open(yes, no, true, text, displays, amounts, true, false, null);
    }
}
