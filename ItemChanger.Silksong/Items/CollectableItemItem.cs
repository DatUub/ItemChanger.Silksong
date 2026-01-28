using ItemChanger.Items;

namespace ItemChanger.Silksong.Items
{
    public class CollectableItemItem : Item
    {
        public required string CollectableName { get; init; }

        public override void GiveImmediate(GiveInfo info)
        {
            var collectableItem = CollectableItemManager.GetItemByName(CollectableName);
            
            if (collectableItem == null)
            {
                Logger.LogError($"CollectableItemItem: Could not find collectable '{CollectableName}'");
                return;
            }

            collectableItem.Collect(1, showPopup: false);
        }

        public override bool Redundant()
        {
            var collectableItem = CollectableItemManager.GetItemByName(CollectableName);
            
            return collectableItem?.IsAtMax() ?? false;
        }
    }
}
