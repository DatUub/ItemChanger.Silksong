using BepInEx;

namespace ItemChanger.Silksong
{
    [BepInDependency("io.github.benchwarp.silksong")]
    [BepInAutoPlugin(id: "io.github.silksong.itemchanger")]
    public partial class ItemChangerPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Logger.LogInfo($"Plugin {Name} ({Id}) has loaded!");
            StartCoroutine(WaitToDo());
        }

        private System.Collections.IEnumerator WaitToDo()
        {
            while (true)
            {
                try
                {
                    yield break;
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
                yield return null;
            }
        }
    }
}