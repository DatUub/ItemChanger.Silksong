namespace ItemChangerTesting.LocationTests;

internal static class MooshkaTestHelpers
{
    // Vanilla SavedFlea_ PD bools, source: Assembly-CSharp strings dump.
    public static readonly string[] VanillaFleaPDBools =
    [
        "SavedFlea_Ant_03",
        "SavedFlea_Belltown_04",
        "SavedFlea_Bone_06",
        "SavedFlea_Bone_East_05",
        "SavedFlea_Bone_East_10_Church",
        "SavedFlea_Bone_East_17b",
        "SavedFlea_Coral_24",
        "SavedFlea_Coral_35",
        "SavedFlea_Crawl_06",
        "SavedFlea_Dock_03d",
        "SavedFlea_Dock_16",
        "SavedFlea_Dust_09",
        "SavedFlea_Dust_12",
        "SavedFlea_Greymoor_06",
        "SavedFlea_Greymoor_15b",
        "SavedFlea_Library_01",
        "SavedFlea_Library_09",
        "SavedFlea_Peak_05c",
        "SavedFlea_Shadow_10",
        "SavedFlea_Shadow_28",
        "SavedFlea_Shellwood_03",
        "SavedFlea_Slab_06",
        "SavedFlea_Slab_Cell",
        "SavedFlea_Song_11",
        "SavedFlea_Under_21",
        "SavedFlea_Under_23",
    ];

    // SavedFleasCount is a getter; the only way to fake-rescue N fleas is to flip N of the
    // underlying SavedFlea_ bools directly. Flips the first `count` entries.
    public static void SetVanillaFleaCount(int count)
    {
        for (int i = 0; i < VanillaFleaPDBools.Length; i++)
        {
            PlayerData.instance.SetBool(VanillaFleaPDBools[i], i < count);
        }
    }
}
