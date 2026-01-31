using ItemChanger.Items;
using ItemChanger.Silksong;

namespace ItemChanger.Silksong.Items
{
    public sealed class MarkerItem : Item
    {
        public required string MarkerFieldName { get; init; }

        public override void GiveImmediate(GiveInfo info)
        {
            var pd = GameManager.instance?.playerData;
            if (pd == null)
            {
                Logger.LogError($"MarkerItem: PlayerData is null");
                return;
            }

            PlayerDataAccessor.SetBool(pd, "hasMarker", true);
            PlayerDataAccessor.SetBool(pd, MarkerFieldName, true);
        }

        public override bool Redundant()
        {
            var pd = GameManager.instance?.playerData;
            return pd != null && PlayerDataAccessor.GetBool(pd, MarkerFieldName);
        }
    }
}
