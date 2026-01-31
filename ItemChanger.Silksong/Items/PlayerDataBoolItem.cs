using ItemChanger.Items;
using ItemChanger.Silksong;

namespace ItemChanger.Silksong.Items
{
    public sealed class PlayerDataBoolItem : Item
    {
        public required string FieldName { get; init; }

        public override void GiveImmediate(GiveInfo info)
        {
            var pd = GameManager.instance?.playerData;
            if (pd == null)
            {
                Logger.LogError($"PlayerDataBoolItem: PlayerData is null");
                return;
            }

            PlayerDataAccessor.SetBool(pd, FieldName, true);
        }

        public override bool Redundant()
        {
            var pd = GameManager.instance?.playerData;
            return pd != null && PlayerDataAccessor.GetBool(pd, FieldName);
        }
    }
}
