using ItemChanger.Items;
using ItemChanger.Serialization;
using ItemChanger.Silksong.Items;
using ItemChanger.Silksong.Serialization;
using ItemChanger.Silksong.UIDefs;

namespace ItemChanger.Silksong.RawData;

internal static partial class BaseItemList
{
    public static Item Flea => new FleaItem
    {
        Name = ItemNames.Flea,
        UIDef = new MsgUIDef()
        {
            // TODO - improve the shopdesc
            Name = new CountedString() { Prefix = new LanguageString("UI", "KEY_FLEA"), Amount = new FleaCount() },
            Sprite = new FleaSprite(),
            ShopDesc = new BoxedString("Flea flea flea flea flea"),
            PreviewName = new LanguageString("UI", "KEY_FLEA")
        },
    };

        // Resources (stat upgrades)
        public static Item Silk_Heart => new PlayerDataIntItem { Name = ItemNames.Silk_Heart, FieldName = "silkRegenMax" };
        public static Item Mask_Shard => new MaskShardItem { Name = ItemNames.Mask_Shard };
        public static Item Spool_Fragment => new SpoolFragmentItem { Name = ItemNames.Spool_Fragment };
        public static Item Tool_Pouch => new PlayerDataIntItem { Name = ItemNames.Tool_Pouch, FieldName = "ToolPouchUpgrades" };
        public static Item Crafting_Kit => new PlayerDataIntItem { Name = ItemNames.Crafting_Kit, FieldName = "ToolKitUpgrades" };

        // Vesticrests
        public static Item Vesticrest_Blue_Expansion => new PDBoolItem { Name = "Vesticrest_Blue-Expansion", BoolName = "UnlockedExtraBlueSlot" };
        public static Item Vesticrest_Yellow => new PDBoolItem { Name = "Vesticrest_Yellow", BoolName = "UnlockedExtraYellowSlot" };

        // Keys (PlayerData)
        public static Item Key_of_Apostate => new PDBoolItem { Name = "Key_of_Apostate", BoolName = "HasSlabKeyA" };
        public static Item Key_of_Heretic => new PDBoolItem { Name = "Key_of_Heretic", BoolName = "HasSlabKeyB" };
        public static Item Key_of_Indolent => new PDBoolItem { Name = "Key_of_Indolent", BoolName = "HasSlabKeyC" };

        // Melodies
        public static Item Architect_s_Melody => new PDBoolItem { Name = "Architect's_Melody", BoolName = "HasMelodyArchitect" };
        public static Item Conductor_s_Melody => new PDBoolItem { Name = "Conductor's_Melody", BoolName = "HasMelodyConductor" };
        public static Item Vaultkeeper_s_Melody => new PDBoolItem { Name = "Vaultkeeper's_Melody", BoolName = "HasMelodyLibrarian" };

    public static Dictionary<string, Item> GetBaseItems()
    {
        return typeof(BaseItemList).GetProperties().Select(p => (Item)p.GetValue(null)).ToDictionary(i => i.Name);
    }
}
