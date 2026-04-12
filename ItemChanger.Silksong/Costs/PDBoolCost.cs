using ItemChanger.Costs;
using ItemChanger.Serialization;
using ItemChanger.Silksong.Serialization;
using System;
using UnityEngine;

namespace ItemChanger.Silksong.Costs
{
    public class PDBoolCost(string boolName, IValueProvider<string> costText, IValueProvider<Sprite>? sprite = null) : ThresholdBoolCost, IDisplayCost
    {
        public string BoolName { get; init; } = boolName;
        public IValueProvider<string> CostText { get; init; } = costText;
        public IValueProvider<Sprite>? Sprite { get; init; } = sprite;

        public override string GetCostText()
        {
            return CostText?.Value ?? $"Have set {BoolName}";  // This should never be displayed, so we shouldn't need to bother localizing
        }

        protected override IValueProvider<bool> GetValueSource()
        {
            return new PDBool(BoolName);
        }

        int IDisplayCost.Amount => CanPay() ? 1 : 0;
        bool IDisplayCost.DisplayEnabled => Sprite is not null;
        Sprite IDisplayCost.DisplaySprite => Sprite?.Value ?? new EmptySprite().Value;
    }
}
