using ItemChanger.Costs;
using ItemChanger.Serialization;
using System;

namespace ItemChanger.Silksong.Costs
{
    public record PDBoolCost(string BoolName) : ThresholdBoolCost
    {
        public override string GetCostText()
        {
            throw new NotImplementedException();
        }

        protected override IBool GetValueSource()
        {
            throw new NotImplementedException();
        }
    }
}
