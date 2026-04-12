using ItemChanger.Costs;
using ItemChanger.Silksong.Costs;
using UnityEngine;

namespace ItemChanger.Silksong.Components;

public class ItemChangerCostOwner : SavedItem
{
    public Cost Cost;

    public static ItemChangerCostOwner FromCost(Cost cost)
    {
        ItemChangerCostOwner owner = ScriptableObject.CreateInstance<ItemChangerCostOwner>();
        owner.Cost = cost;
        return owner;
    }

    public override bool CanGetMore()
    {
        return true;
    }

    public override void Get(bool showPopup = true)
    {
        Cost.Pay();
    }

    // CanPay is effectively equivalent to GetSavedAmount >= the number passed to DialogueYNBox.Open
    public override int GetSavedAmount()
    {
        if (Cost is IDisplayCost displayCost && displayCost.DisplayEnabled)
        {
            return displayCost.Amount;
        }

        return Cost.CanPay() ? 1 : 0;
    }

    public override Sprite? GetPopupIcon()
    {
        return Cost is IDisplayCost displayCost && displayCost.DisplayEnabled ? displayCost.DisplaySprite : null;
    }
}
