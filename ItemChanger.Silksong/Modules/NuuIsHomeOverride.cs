using Benchwarp.Data;
using ItemChanger.Modules;
using PrepatcherPlugin;
using Silksong.UnityHelper.Extensions;
using UnityEngine.SceneManagement;

namespace ItemChanger.Silksong.Modules;

/// <summary>
/// Changes Nuu behavior so that Nuu is present in Halfway House until final departure,
/// and so that Nuu's scroll is interactable until final departure.
/// </summary>
[SingletonModule]
public class NuuIsHomeOverride : Module
{
    protected override void DoLoad()
    {
        PlayerDataVariableEvents.OnGetBool += OverrideGetBool;
        Using(new SceneEditGroup { { SceneNames.Halfway_01, ModifyScene } });
    }
    protected override void DoUnload()
    {
        PlayerDataVariableEvents.OnGetBool -= OverrideGetBool;
    }
    private bool OverrideGetBool(PlayerData pd, string fieldName, bool current) => fieldName == nameof(PlayerData.nuuIsHome) || current;
    private void ModifyScene(Scene scene)
    {
        GameObject? inspectRegion = scene.FindGameObject("_NPCs/Hunter Fan Control/Nuu_Scrolls/Inspect Region");
        if (inspectRegion == null)
        {
            LogWarn($"{GetType().Name} failed to find Nuu scroll inspect region.");
            return;
        }
        inspectRegion.RemoveComponent<DeactivateIfPlayerdataTrue>();
        inspectRegion.SetActive(true);
    }
}
