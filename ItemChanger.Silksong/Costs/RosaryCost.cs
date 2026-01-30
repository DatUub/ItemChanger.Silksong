using ItemChanger.Costs;
using TeamCherry.Localization;

namespace ItemChanger.Silksong.Costs
{
    public record RosaryCost(int Amount) : Cost
    {
        /// <summary>
        /// Amount after accounting for any discount rate.
        /// </summary>
        public int ActualAmount => (int)(Amount * base.DiscountRate);

        // Optimization: Access geo directly to avoid string lookup overhead in GetInt
        public override bool CanPay() => PlayerData.instance.geo >= ActualAmount;

        public override string GetCostText() => string.Format(Language.Get("PAY_ROSARIES", "Fmt"), ActualAmount);
        
        public override bool HasPayEffects() => true;

        public override void OnPay()
        {
            if (ActualAmount > 0) HeroController.instance.TakeGeo(ActualAmount);
        }

        public override bool Includes(Cost c) => base.Includes(c) || (c is RosaryCost rc && rc.ActualAmount <= ActualAmount);
    }
}
