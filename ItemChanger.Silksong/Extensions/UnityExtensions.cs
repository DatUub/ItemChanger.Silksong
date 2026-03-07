using ItemChanger.Extensions;
using Silksong.AssetHelper.ManagedAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Silksong.Extensions;

public static class UnityExtensions
{
    public static GameObject InstantiateInScene(this ManagedAsset<GameObject> asset, Scene scene)
        => scene.Instantiate(asset.Handle.Result);

    public static void DoNextFrame(this MonoBehaviour b, Action? a)
    {
        b.StartCoroutine(NextFrame(a));
    }

    private static IEnumerator NextFrame(Action? a)
    {
        yield return null;
        try
        {
            a?.Invoke();
        }
        catch (Exception e)
        {
            GlobalRefs.Logger.LogError(e.ToString());
            throw;
        }
    }

}
