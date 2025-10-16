using ItemChanger.Items;

namespace ItemChanger.Silksong.Items
{
    public class PDBoolItem : Item
    {
        public required string BoolName { get; init; }
        public bool SetValue { get; init; } = true;

        public override void GiveImmediate(GiveInfo info)
        {
            PlayerData.instance.SetBool(BoolName, SetValue);
        }

        public override bool Redundant()
        {
            return PlayerData.instance.GetBool(BoolName) == SetValue;
        }
    }
}
