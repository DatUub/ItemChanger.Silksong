using BepInEx;

namespace ItemChanger.Silksong
{
    [BepInDependency("io.github.benchwarp")]
    [BepInAutoPlugin(id: "io.github.silksong.itemchanger")]
    public partial class ItemChangerPlugin : BaseUnityPlugin
    {
        public static ItemChangerPlugin Instance { get => field ?? throw new NullReferenceException("ItemChangerPlugin is not loaded!"); private set; }
        internal new BepInEx.Logging.ManualLogSource Logger => base.Logger;

        private void Awake()
        {
            Instance = this;
            new SilksongHost();
            Logger.LogInfo($"Plugin {Name} ({Id}) has loaded!");

            
        }


    }
}