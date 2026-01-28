using ItemChanger.Items;

namespace ItemChanger.Silksong.Items
{
    public sealed class CrestItem : Item
    {
        public required string CrestName { get; init; }

        public override void GiveImmediate(GiveInfo info)
        {
            var crest = ToolItemManager.GetCrestByName(CrestName);
            
            if (crest == null)
            {
                Logger.LogError($"CrestItem: Could not find crest '{CrestName}'");
                return;
            }

            crest.Unlock();
            crest.Get(showPopup: false);
        }

        public override bool Redundant()
        {
            var crest = ToolItemManager.GetCrestByName(CrestName);
            
            return crest?.IsUnlocked ?? false;
        }
    }
}
