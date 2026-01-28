using ItemChanger.Items;
using ItemChanger.Silksong.Items;
using static ItemChanger.Silksong.RawData.ItemNames;

namespace ItemChanger.Silksong.RawData
{
    internal static class BaseItemList
    {
        public static IEnumerable<Item> GetItems()
        {
            // Silk skills
            yield return new ToolItemItem { Name = Cross_Stitch, ToolName = "Silk Bomb" };
            yield return new ToolItemItem { Name = Pale_Nails, ToolName = "Silk Boss Needle" };
            yield return new ToolItemItem { Name = Rune_Rage, ToolName = "Silk Charge" };
            yield return new ToolItemItem { Name = Sharpdart, ToolName = "Parry" };
            yield return new ToolItemItem { Name = Silkspear, ToolName = "Silk Spear" };
            yield return new ToolItemItem { Name = Thread_Storm, ToolName = "Thread Sphere" };

            // Ancestral arts
            yield return new PlayerDataBoolItem { Name = Beastling_Call, FieldName = "hasBeastlingCall" };
            yield return new PlayerDataBoolItem { Name = Clawline, FieldName = "hasClawline" };
            yield return new PlayerDataBoolItem { Name = Cling_Grip, FieldName = "hasClingGrip" };
            yield return new PlayerDataBoolItem { Name = Elegy_of_the_Deep, FieldName = "hasElegyOfTheDeep" };
            yield return new PlayerDataBoolItem { Name = Needolin, FieldName = "hasNeedolin" };
            yield return new PlayerDataBoolItem { Name = Silk_Soar, FieldName = "hasSilkSoar" };
            yield return new PlayerDataBoolItem { Name = Swift_Step, FieldName = "hasSwiftStep" };
            yield return new PlayerDataBoolItem { Name = Sylphsong, FieldName = "hasSylphsong" };

            // other abilities
            yield return new PlayerDataBoolItem { Name = Bind, FieldName = "hasBind" };
            yield return new PlayerDataBoolItem { Name = Drifter_s_Cloak, FieldName = "hasDriftersCloak" };
            yield return new PlayerDataBoolItem { Name = Faydown_Cloak, FieldName = "hasFaydownCloak" };
            yield return new PlayerDataBoolItem { Name = Needle_Strike, FieldName = "hasNeedleStrike" };

            // plot items (note: snare setter listed with red tools)
            yield return new CollectableItemItem { Name = Architect_s_Melody, CollectableName = "Architect's Melody" };
            yield return new CollectableItemItem { Name = Conductor_s_Melody, CollectableName = "Conductor's Melody" };
            yield return new CollectableItemItem { Name = Conjoined_Heart, CollectableName = "Coral Heart" };
            yield return new CollectableItemItem { Name = Encrusted_Heart, CollectableName = "Clover Heart" };
            yield return new CollectableItemItem { Name = Everbloom, CollectableName = "Everbloom" };
            yield return new CollectableItemItem { Name = Hermit_s_Soul, CollectableName = "Snare Soul Bell Hermit" };
            yield return new CollectableItemItem { Name = Hunter_s_Heart, CollectableName = "Hunter Heart" };
            yield return new CollectableItemItem { Name = Maiden_s_Soul, CollectableName = "Snare Soul Churchkeeper" };
            yield return new CollectableItemItem { Name = Pollen_Heart, CollectableName = "Flower Heart" };
            yield return new CollectableItemItem { Name = Seeker_s_Soul, CollectableName = "Snare Soul Swamp Bug" };
            yield return new CollectableItemItem { Name = Vaultkeeper_s_Melody, CollectableName = "Vaultkeeper's Melody" };

            // crests
            yield return new CrestItem { Name = Crest_of_Architect, CrestName = "Toolmaster" };
            yield return new CrestItem { Name = Crest_of_Beast, CrestName = "Warrior" };
            yield return new CrestItem { Name = Crest_of_Cloakless, CrestName = "Cloakless" };
            yield return new CrestItem { Name = Crest_of_Cursed_Witch, CrestName = "Cursed" };
            yield return new CrestItem { Name = Crest_of_Hunter, CrestName = "Hunter" };
            yield return new CrestItem { Name = Crest_of_Hunter__Upgrade_1, CrestName = "Hunter_v2" };
            yield return new CrestItem { Name = Crest_of_Hunter__Upgrade_2, CrestName = "Hunter_v3" };
            yield return new CrestItem { Name = Crest_of_Reaper, CrestName = "Reaper" };
            yield return new CrestItem { Name = Crest_of_Shaman, CrestName = "Spell" };
            yield return new CrestItem { Name = Crest_of_Wanderer, CrestName = "Wanderer" };
            yield return new CrestItem { Name = Crest_of_Witch, CrestName = "Witch" };
            yield return new CrestItem { Name = Vesticrest_Blue__Expansion, CrestName = "Vesticrest_Blue-Expansion" };
            yield return new CrestItem { Name = Vesticrest_Yellow, CrestName = "Vesticrest_Yellow" };

            // memory lockets
            yield return new CollectableItemItem { Name = Memory_Locket, CollectableName = "Crest Socket Unlocker" };

            // red tools (note: ruined tool is listed with quest items)
            yield return new ToolItemItem { Name = Cogfly, ToolName = "Cogwork Flier" };
            yield return new ToolItemItem { Name = Cogwork_Wheel, ToolName = "Cogwork Saw" };
            yield return new ToolItemItem { Name = Conchcutter, ToolName = "Conch Drill" };
            yield return new ToolItemItem { Name = Curveclaw, ToolName = "Curve Claws" };
            yield return new ToolItemItem { Name = Curvesickle, ToolName = "Curve Claws Upgraded" };
            yield return new ToolItemItem { Name = Delver_s_Drill, ToolName = "Extractor" };
            yield return new ToolItemItem { Name = Flea_Brew, ToolName = "Flea Brew" };
            yield return new ToolItemItem { Name = Flintslate, ToolName = "Flintstone" };
            yield return new ToolItemItem { Name = Longpin, ToolName = "Harpoon" };
            yield return new ToolItemItem { Name = Needle_Phial, ToolName = "Lifeblood Syringe" };
            yield return new ToolItemItem { Name = Pimpillo, ToolName = "Pimpilo" };
            yield return new ToolItemItem { Name = Plasmium_Phial, ToolName = "Plasmium_Phial" };
            yield return new ToolItemItem { Name = Rosary_Cannon, ToolName = "Rosary Cannon" };
            yield return new ToolItemItem { Name = Silkshot__Forge_Daughter, ToolName = "WebShot Forge" };
            yield return new ToolItemItem { Name = Silkshot__Mount_Fay, ToolName = "Silkshot-Mount_Fay" };
            yield return new ToolItemItem { Name = Silkshot__Original, ToolName = "WebShot Weaver" };
            yield return new ToolItemItem { Name = Silkshot__Twelfth_Architect, ToolName = "WebShot Architect" };
            yield return new ToolItemItem { Name = Snare_Setter, ToolName = "Silk Snare" };
            yield return new ToolItemItem { Name = Sting_Shard, ToolName = "Sting Shard" };
            yield return new ToolItemItem { Name = Straight_Pin, ToolName = "Straight Pin" };
            yield return new ToolItemItem { Name = Tacks, ToolName = "Tack" };
            yield return new ToolItemItem { Name = Threefold_Pin, ToolName = "Tri Pin" };
            yield return new ToolItemItem { Name = Throwing_Ring, ToolName = "Shakra Ring" };
            yield return new ToolItemItem { Name = Voltvessels, ToolName = "Lightning Rod" };

            // blue tools
            yield return new ToolItemItem { Name = Claw_Mirror, ToolName = "Dazzle Bind" };
            yield return new ToolItemItem { Name = Claw_Mirrors, ToolName = "Dazzle Bind Upgraded" };
            yield return new ToolItemItem { Name = Druid_s_Eye, ToolName = "Druid Eye" };
            yield return new ToolItemItem { Name = Druid_s_Eyes, ToolName = "Druid's_Eyes" };
            yield return new ToolItemItem { Name = Egg_of_Flealia, ToolName = "Flea Charm" };
            yield return new ToolItemItem { Name = Fractured_Mask, ToolName = "Fractured Mask" };
            yield return new ToolItemItem { Name = Injector_Band, ToolName = "Injector Band" };
            yield return new ToolItemItem { Name = Longclaw, ToolName = "Brolly Spike" };
            yield return new ToolItemItem { Name = Magma_Bell, ToolName = "Lava Charm" };
            yield return new ToolItemItem { Name = Memory_Crystal, ToolName = "Revenge Crystal" };
            yield return new ToolItemItem { Name = Multibinder, ToolName = "Bell Bind" };
            yield return new ToolItemItem { Name = Pin_Badge, ToolName = "Pinstress Tool" };
            yield return new ToolItemItem { Name = Pollip_Pouch, ToolName = "Poison Pouch" };
            yield return new ToolItemItem { Name = Quick_Sling, ToolName = "Quick Sling" };
            yield return new ToolItemItem { Name = Reserve_Bind, ToolName = "Reserve Bind" };
            yield return new ToolItemItem { Name = Sawtooth_Circlet, ToolName = "Screw Attack" };
            yield return new ToolItemItem { Name = Snitch_Pick, ToolName = "Thief Claw" };
            yield return new ToolItemItem { Name = Spool_Extender, ToolName = "Spool Extender" };
            yield return new ToolItemItem { Name = Volt_Filament, ToolName = "Zap Imbuement" };
            yield return new ToolItemItem { Name = Warding_Bell, ToolName = "Warding Bell" };
            yield return new ToolItemItem { Name = Weavelight, ToolName = "Weavelight" };
            yield return new ToolItemItem { Name = Wispfire_Lantern, ToolName = "Wisp Lantern" };
            yield return new ToolItemItem { Name = Wreath_of_Purity, ToolName = "White Ring" };

            // yellow tools
            yield return new ToolItemItem { Name = Ascendant_s_Grip, ToolName = "Wallcling" };
            yield return new ToolItemItem { Name = Barbed_Bracelet, ToolName = "Barbed Wire" };
            yield return new ToolItemItem { Name = Compass, ToolName = "Compass" };
            yield return new ToolItemItem { Name = Dead_Bug_s_Purse, ToolName = "Dead Mans Purse" };
            yield return new ToolItemItem { Name = Magnetite_Brooch, ToolName = "Rosary Magnet" };
            yield return new ToolItemItem { Name = Magnetite_Dice, ToolName = "Magnetite Dice" };
            yield return new ToolItemItem { Name = Scuttlebrace, ToolName = "Scuttlebrace" };
            yield return new ToolItemItem { Name = Shard_Pendant, ToolName = "Shard Pendant" };
            yield return new ToolItemItem { Name = Shell_Satchel, ToolName = "Shell Satchel" };
            yield return new ToolItemItem { Name = Silkspeed_Anklets, ToolName = "Sprintmaster" };
            yield return new ToolItemItem { Name = Spider_Strings, ToolName = "Spider Strings" };
            yield return new ToolItemItem { Name = Thief_s_Mark, ToolName = "Thief Charm" };
            yield return new ToolItemItem { Name = Weighted_Belt, ToolName = "Weighted Belt" };
            yield return new CollectableItemItem { Name = Tool_Pouch, CollectableName = "Tool Pouch&Kit Inv" };
            yield return new CollectableItemItem { Name = Crafting_Kit, CollectableName = "Tool Pouch&Kit Inv" };

            // Resources
            yield return new CollectableItemItem { Name = Craftmetal, CollectableName = "Tool Metal" };
            yield return new CollectableItemItem { Name = Silk_Heart, CollectableName = "Silk Heart" };
            yield return new CollectableItemItem { Name = Pale_Oil, CollectableName = "Pale_Oil" };
            yield return new CollectableItemItem { Name = Mask_Shard, CollectableName = "Great Shard" };
            yield return new CollectableItemItem { Name = Spool_Fragment, CollectableName = "Shard Pouch" };

            // shell shard consumables (currency items that break into shell shards)
            yield return new CollectableItemItem { Name = Shard_Bundle, CollectableName = "Shard_Bundle" }; // 80 shards
            yield return new CollectableItemItem { Name = Beast_Shard, CollectableName = "Beastfly Remains" }; // 140 shards
            yield return new CollectableItemItem { Name = Pristine_Core, CollectableName = "Pristine Core" }; // 220 shards
            yield return new CollectableItemItem { Name = Growstone, CollectableName = "Growstone" };
            yield return new CollectableItemItem { Name = Hornet_Statuette, CollectableName = "Fixer Idol" };

            // shell shard breakables (destructible objects that drop shards)
            yield return new CollectableItemItem { Name = Shell_Shard_Fossil, CollectableName = "Shell_Shard_Fossil" };

            // silkeaters (cocoon recovery grubs)
            yield return new CollectableItemItem { Name = Silkeater, CollectableName = "Silk Grub" };

            // keys
            yield return new CollectableItemItem { Name = Architect_s_Key, CollectableName = "Architect Key" };
            yield return new CollectableItemItem { Name = Bellhome_Key, CollectableName = "Belltown House Key" };
            yield return new CollectableItemItem { Name = Craw_Summons, CollectableName = "Craw Summons" };
            yield return new CollectableItemItem { Name = Diving_Bell_Key, CollectableName = "Dock Key" };
            yield return new CollectableItemItem { Name = Key_of_Apostate, CollectableName = "Ward Key" };
            yield return new CollectableItemItem { Name = Key_of_Heretic, CollectableName = "Ward Boss Key" };
            yield return new CollectableItemItem { Name = Key_of_Indolent, CollectableName = "Slab Key" };
            yield return new CollectableItemItem { Name = Simple_Key, CollectableName = "Simple Key" };
            yield return new CollectableItemItem { Name = Surgeon_s_Key, CollectableName = "Surgeon's Key" };
            yield return new CollectableItemItem { Name = White_Key, CollectableName = "White Key" };

            // relics
            //yield return new CollectableItemItem { Name = Arcane_Egg, CollectableName = "R Ancient Egg" };
            //yield return new CollectableItemItem { Name = Bone_Scroll__Burning_Bug, CollectableName = "R Bone Record" };
            //yield return new CollectableItemItem { Name = Rune_Harp__Burden, CollectableName = "R Weaver Record" };
            //yield return new CollectableItemItem { Name = Choral_Commandment__Light, CollectableName = "R Seal Chit" };
            //yield return new CollectableItemItem { Name = Weaver_Effigy__Atla, CollectableName = "R Weaver Totem" };
            //yield return new CollectableItemItem { Name = Psalm_Cylinder__Sermon, CollectableName = "R Psalm Cylinder" };
            //yield return new CollectableItemItem { Name = Sacred_Cylinder, CollectableName = "R Librarian Melody Cylinder" };

            // maps
            yield return new PlayerDataBoolItem { Name = Bellhart_Map, FieldName = "HasBellhartMap" };
            yield return new PlayerDataBoolItem { Name = Bilewater_Map, FieldName = "HasSwampMap" };
            yield return new PlayerDataBoolItem { Name = Blasted_Steps_Map, FieldName = "HasJudgeStepsMap" };
            yield return new PlayerDataBoolItem { Name = Choral_Chambers_Map, FieldName = "HasHallsMap" };
            yield return new PlayerDataBoolItem { Name = Cogwork_Core_Map, FieldName = "HasCogMap" };
            yield return new PlayerDataBoolItem { Name = Cradle_Map, FieldName = "HasCradleMap" };
            yield return new PlayerDataBoolItem { Name = Deep_Docks_Map, FieldName = "HasDocksMap" };
            yield return new PlayerDataBoolItem { Name = Far_Fields_Map, FieldName = "HasWildsMap" };
            yield return new PlayerDataBoolItem { Name = Grand_Gate_Map, FieldName = "HasSongGateMap" };
            yield return new PlayerDataBoolItem { Name = Greymoor_Map, FieldName = "HasGreymoorMap" };
            yield return new PlayerDataBoolItem { Name = High_Halls_Map, FieldName = "HasHangMap" };
            yield return new PlayerDataBoolItem { Name = Hunter_s_March_Map, FieldName = "HasHuntersNestMap" };
            yield return new PlayerDataBoolItem { Name = Memorium_Map, FieldName = "HasArboriumMap" };
            yield return new PlayerDataBoolItem { Name = Mosslands_Map, FieldName = "HasMossGrottoMap" };
            yield return new PlayerDataBoolItem { Name = Mount_Fay_Map, FieldName = "HasPeakMap" };
            yield return new PlayerDataBoolItem { Name = Putrified_Ducts_Map, FieldName = "HasAqueductMap" };
            yield return new PlayerDataBoolItem { Name = Sands_of_Karak_Map, FieldName = "HasCoralMap" };
            yield return new PlayerDataBoolItem { Name = Shellwood_Map, FieldName = "HasShellwoodMap" };
            yield return new PlayerDataBoolItem { Name = Sinner_s_Road_Map, FieldName = "HasDustpensMap" };
            yield return new PlayerDataBoolItem { Name = The_Abyss_Map, FieldName = "HasAbyssMap" };
            yield return new PlayerDataBoolItem { Name = The_Marrow_Map, FieldName = "HasBoneforestMap" };
            yield return new PlayerDataBoolItem { Name = The_Slab_Map, FieldName = "HasSlabMap" };
            yield return new PlayerDataBoolItem { Name = Underworks_Map, FieldName = "HasCitadelUnderstoreMap" };
            yield return new PlayerDataBoolItem { Name = Verdania_Map, FieldName = "HasCloverMap" };
            yield return new PlayerDataBoolItem { Name = Weavenest_Alta_Map, FieldName = "HasWeavehomeMap" };
            yield return new PlayerDataBoolItem { Name = Whispering_Vaults_Map, FieldName = "HasLibraryMap" };
            yield return new PlayerDataBoolItem { Name = Whiteward_Map, FieldName = "HasWardMap" };
            yield return new PlayerDataBoolItem { Name = Wormways_Map, FieldName = "HasCrawlMap" };

            // quills
            yield return new PlayerDataBoolItem { Name = Quill__Purple, FieldName = "hasQuill" };

            // yield return new PlayerDataBoolItem { Name = Quill__White, FieldName = "hasQuill" };

            // map markers
            yield return new MarkerItem { Name = Bronze_Marker, MarkerFieldName = "hasMarker_e" };
            yield return new MarkerItem { Name = Dark_Marker, MarkerFieldName = "hasMarker_d" };
            yield return new MarkerItem { Name = Hunt_Marker, MarkerFieldName = "hasMarker_c" };
            yield return new MarkerItem { Name = Ring_Marker, MarkerFieldName = "hasMarker_b" };
            yield return new MarkerItem { Name = Shell_Marker, MarkerFieldName = "hasMarker_a" };

            // map pins
            yield return new PlayerDataBoolItem { Name = Bench_Pins, FieldName = "hasPinBench" };
            yield return new PlayerDataBoolItem { Name = Bellway_Pins, FieldName = "hasPinStag" };
            yield return new PlayerDataBoolItem { Name = Vendor_Pins, FieldName = "hasPinShop" };
            yield return new PlayerDataBoolItem { Name = Ventrica_Pins, FieldName = "hasPinTube" };
            yield return new PlayerDataBoolItem { Name = Spa_Pins, FieldName = "hasPinSpa" };
            yield return new PlayerDataBoolItem { Name = Flea_Pins_Marrowlands, FieldName = "hasPinFleaMarrowlands" };
            yield return new PlayerDataBoolItem { Name = Flea_Pins_Midlands, FieldName = "hasPinFleaMidlands" };
            yield return new PlayerDataBoolItem { Name = Flea_Pins_Blastedlands, FieldName = "hasPinFleaBlastedlands" };
            yield return new PlayerDataBoolItem { Name = Flea_Pins_Citadel, FieldName = "hasPinFleaCitadel" };
            yield return new PlayerDataBoolItem { Name = Flea_Pins_Peaklands, FieldName = "hasPinFleaPeaklands" };
            yield return new PlayerDataBoolItem { Name = Flea_Pins_Mucklands, FieldName = "hasPinFleaMucklands" };

            // rosary items
            // Rosary_Bell, Rosary_Bowl, Rosary_Cache, Rosary_Chest, Rosary_Event,
            // Rosary_Grave, Rosary_Npc, Rosary_Pouch, Rosary_Rock, Rosary_Tray

            // consumable rosaries
            yield return new CollectableItemItem { Name = Frayed_Rosary_String, CollectableName = "Rosary_Set_Frayed" };
            yield return new CollectableItemItem { Name = Heavy_Rosary_Necklace, CollectableName = "Rosary_Set_Large" };
            yield return new CollectableItemItem { Name = Pale_Rosary_Necklace, CollectableName = "Rosary_Set_Huge_White" };
            yield return new CollectableItemItem { Name = Rosary_Necklace, CollectableName = "Rosary_Set_Medium" };
            yield return new CollectableItemItem { Name = Rosary_String, CollectableName = "Rosary_Set_Small" };

            // mementos (note: hearts listed with plot items)
            yield return new CollectableItemItem { Name = Craw_Memento, CollectableName = "Memento Seth" };
            yield return new CollectableItemItem { Name = Grey_Memento, CollectableName = "Grey Memento" };
            yield return new CollectableItemItem { Name = Guardian_s_Memento, CollectableName = "Memento Garmond" };
            yield return new CollectableItemItem { Name = Hero_s_Memento, CollectableName = "Crowman Memento" };
            yield return new CollectableItemItem { Name = Hunter_s_Memento, CollectableName = "Hunter Memento" };
            yield return new CollectableItemItem { Name = Sprintmaster_Memento, CollectableName = "Sprintmaster Memento" };
            yield return new CollectableItemItem { Name = Surface_Memento, CollectableName = "Memento Surface" };

            // bellhome upgrades
            yield return new CollectableItemItem { Name = Crawbell, CollectableName = "Crawbell" };
            yield return new CollectableItemItem { Name = Farsight, CollectableName = "Farsight" };
            yield return new CollectableItemItem { Name = Materium, CollectableName = "Materium" };
            // Bell_Lacquer variants, Desk, Gleamlights, Gramophone, Personal_Spa - not yet implemented
            // Materium types (Flintstone, Roach_Guts, Voltridian, Magnetite) - need specific implementation

            // tradables and deliverables
            yield return new CollectableItemItem { Name = Broodmother_s_Eye, CollectableName = "Broodmother Remains" };
            yield return new CollectableItemItem { Name = Crustnut, CollectableName = "Crustnut" };
            yield return new CollectableItemItem { Name = Crow_Feathers, CollectableName = "Crow Feather" };
            yield return new CollectableItemItem { Name = Flintgem, CollectableName = "Smeltstone" };
            yield return new CollectableItemItem { Name = Grass_Doll, CollectableName = "Wood Witch Item" };
            yield return new CollectableItemItem { Name = Horn_Fragment, CollectableName = "Rock Roller Item" };
            yield return new CollectableItemItem { Name = Mossberry_Stew, CollectableName = "Mossberry Stew" };
            yield return new CollectableItemItem { Name = Pickled_Muckmaggot, CollectableName = "Pickled Roach Egg" };
            yield return new CollectableItemItem { Name = Steel_Spines, CollectableName = "Common Spine" };
            yield return new CollectableItemItem { Name = Twisted_Bud, CollectableName = "Shell Flower" };

            // respawning quest drops
            yield return new CollectableItemItem { Name = Choir_Cloak, CollectableName = "Song Pilgrim Cloak" };
            yield return new CollectableItemItem { Name = Fine_Pin, CollectableName = "Fine Pin" };
            yield return new CollectableItemItem { Name = Pilgrim_Shawl, CollectableName = "Pilgrim Rag" };
            yield return new CollectableItemItem { Name = Plasmified_Blood, CollectableName = "Plasmium Blood" };
            yield return new CollectableItemItem { Name = Plasmium, CollectableName = "Plasmium" };
            yield return new CollectableItemItem { Name = Ragpelt, CollectableName = "Ragpelt" };
            yield return new CollectableItemItem { Name = Roach_Guts, CollectableName = "Roach Corpse Item" };
            yield return new CollectableItemItem { Name = Seared_Organ, CollectableName = "Enemy Morsel Seared" };
            yield return new CollectableItemItem { Name = Shredded_Organ, CollectableName = "Enemy Morsel Shredded" };
            yield return new CollectableItemItem { Name = Silver_Bell, CollectableName = "Silver Bellclapper" };
            yield return new CollectableItemItem { Name = Skewered_Organ, CollectableName = "Enemy Morsel Speared" };
            yield return new CollectableItemItem { Name = Spine_Core, CollectableName = "Spine_Core" };

            // deliveries
            yield return new CollectableItemItem { Name = Courier_s_Rasher, CollectableName = "Courier Supplies Gourmand" };
            yield return new CollectableItemItem { Name = Courier_s_Swag, CollectableName = "Courier Supplies" };
            yield return new CollectableItemItem { Name = Liquid_Lacquer, CollectableName = "Courier Supplies Mask Maker" };
            yield return new CollectableItemItem { Name = Queen_s_Egg, CollectableName = "Courier Supplies Slave" };

            // other quest items
            yield return new CollectableItemItem { Name = Cogheart_Piece, CollectableName = "Cog Heart Pieces" };
            yield return new CollectableItemItem { Name = Crown_Fragment, CollectableName = "Skull King Fragment" };
            yield return new CollectableItemItem { Name = Plasmium_Bud, CollectableName = "Plasmium_Bud" };
            yield return new CollectableItemItem { Name = Plasmium_Gland, CollectableName = "Plasmium Gland" };
            yield return new CollectableItemItem { Name = Ruined_Tool, CollectableName = "Broken SilkShot" };
            yield return new CollectableItemItem { Name = Vintage_Nectar, CollectableName = "Vintage Nectar" };

            // journal entries
            yield return new CollectableItemItem { Name = Journal_Entry__Void_Tendrils, CollectableName = "Journal_Entry-Void_Tendrils" };
        }
    }
}
