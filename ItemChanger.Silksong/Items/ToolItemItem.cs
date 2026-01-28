using ItemChanger.Items;

namespace ItemChanger.Silksong.Items
{
    public sealed class ToolItemItem : Item
    {
        public required string ToolName { get; init; }

        public override void GiveImmediate(GiveInfo info)
        {
            var tool = ToolItemManager.GetToolByName(ToolName);
            
            if (tool == null)
            {
                Logger.LogError($"ToolItemItem: Could not find tool '{ToolName}'");
                return;
            }

            tool.Get(showPopup: false);
        }

        public override bool Redundant()
        {
            var tool = ToolItemManager.GetToolByName(ToolName);
            
            return tool?.IsUnlocked ?? false;
        }
    }
}
