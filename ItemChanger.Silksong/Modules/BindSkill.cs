using HarmonyLib;
using ItemChanger.Modules;

namespace ItemChanger.Silksong.Modules
{
    public class BindSkill : Module
    {
        public static BindSkill? Instance { get; private set; }

        public bool CanBind { get; set; }

        private Harmony? _harmony;

        protected override void DoLoad()
        {
            Instance = this;
            _harmony = new Harmony($"ItemChanger.Silksong.{nameof(BindSkill)}");
            _harmony.PatchAll(typeof(Patches));
        }

        protected override void DoUnload()
        {
            Instance = null;
            _harmony?.UnpatchSelf();
            _harmony = null;
        }

        [HarmonyPatch]
        private static class Patches
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(HeroController), nameof(HeroController.CanBind))]
            private static void OverrideCanBind(ref bool __result)
            {
                if (Instance != null && !Instance.CanBind)
                    __result = false;
            }
        }
    }
}
