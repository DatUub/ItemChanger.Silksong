using ItemChanger.Items;

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

            pd.SetBool("hasMarker", true);
            pd.SetBool(MarkerFieldName, true);
        }

        public override bool Redundant()
        {
            var pd = GameManager.instance?.playerData;
            return pd?.GetBool(MarkerFieldName) ?? false;
        }
    }
}
