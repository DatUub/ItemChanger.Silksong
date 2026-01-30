using ItemChanger.Costs;
using ItemChanger.Serialization;
using ItemChanger.Silksong.Serialization;
using TeamCherry.Localization;

namespace ItemChanger.Silksong.Costs
{
    public record PDBoolCost(string BoolName) : ThresholdBoolCost
    {
        public override string GetCostText()
        {
            return string.Format(Language.Get("COST_PDBOOL", "Fmt"), BoolName);
        }

        protected override IBool GetValueSource()
        {
            return new PDBool(BoolName);
        }
    }
}
