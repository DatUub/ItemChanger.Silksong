using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Enums;
using ItemChanger.Serialization;
using ItemChanger.Silksong.Assets;
using Silksong.FsmUtil;
using Silksong.UnityHelper.Extensions;
using TMProOld;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

namespace ItemChanger.Silksong.UIDefs;

public class DefaultBigUiDef : CascadingUIDef
{
    public required DefaultBigUiDefData Data { get; init; }

    public override MessageType RequiredMessageType => MessageType.LargePopup;

    public override void DoSendMessage(Action? callback)
    {
        GameObject spawnedMessage = GameObjectKeys.ITEM_GET_PROMPT.InstantiateAsset(SceneManager.GetActiveScene());
        spawnedMessage.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        DefaultBigUiDefDataComponent cpt = spawnedMessage.GetOrAddComponent<DefaultBigUiDefDataComponent>();
        cpt.Data = Data;
        cpt.Callback = callback;

        PlayMakerFSM fsm = spawnedMessage.LocateMyFSM("Msg Control");
        if (fsm.GetState("ItemChanger Custom") is null)
        {
            ModifyFsm(fsm);
            HastenFsm(fsm);
        }
        fsm.FindStringVariable("Item")!.Value = "ItemChanger Custom";
    }

    private void HastenFsm(PlayMakerFSM fsm)
    {
        fsm.MustGetState("Audio Play").GetLastActionOfType<Wait>()!.time.value = 0.25f;
        fsm.MustGetState("Bot Up").GetLastActionOfType<Wait>()!.time.value = 0.25f;
        fsm.MustGetState("Down").GetLastActionOfType<Wait>()!.time.value = 0.25f;
    }

    private void ModifyFsm(PlayMakerFSM fsm)
    {
        FsmState newState = fsm.AddState("ItemChanger Custom");
        newState.AddTransition("FINISHED", "Top Up");
        FsmEvent newEvent = fsm.AddTransition("Init", "ITEMCHANGER_CUSTOM", "ItemChanger Custom");

        StringSwitch sw = fsm.MustGetState("Init").GetFirstActionOfType<StringSwitch>()!;
        sw.compareTo = sw.compareTo.Append("ItemChanger Custom").ToArray();
        sw.sendEvent = sw.sendEvent.Append(newEvent).ToArray();

        newState.AddMethod(static a =>
        {
            GameObject go = a.fsmComponent.gameObject;
            DefaultBigUiDefData? data = go.GetComponent<DefaultBigUiDefDataComponent>().Data;

            if (data?.ActionString is not null)
            {
                go.FindChild("Single Prompt/Button")!.GetComponent<ActionButtonIcon>().SetActionString(data.ActionString);
            }

            foreach ((string objPath, IValueProvider<string> provider) in data?.TextSetters ?? Enumerable.Empty<KeyValuePair<string, IValueProvider<string>>>())
            {
                GameObject? child = go.FindChild(objPath);
                if (child == null)
                {
                    LogWarn($"Did not find child {objPath}");
                }
                else
                {
                    child.GetComponent<TextMeshPro>().text = provider.Value ?? string.Empty;
                }
            }
            foreach ((string objPath, Vector2 offset) in data?.Offsets ?? Enumerable.Empty<KeyValuePair<string, Vector2>>())
            {
                GameObject? child = go.FindChild(objPath);
                if (child == null)
                {
                    LogWarn($"Did not find child {objPath}");
                }
                else
                {
                    child.transform.SetPosition2D((Vector2)child.transform.position + offset);
                }
            }
            foreach (string objPath in data?.Deactivations ?? Enumerable.Empty<string>())
            {
                GameObject? child = go.FindChild(objPath);
                if (child == null)
                {
                    LogWarn($"Did not find child {objPath}");
                }
                else
                {
                    child.SetActive(false);
                }
            }

            if (data?.Sprite?.Value is Sprite sprite && sprite != null)
            {
                go.FindChild("Icon")!.GetComponent<SpriteRenderer>().sprite = sprite;
            }
        });

        fsm.MustGetState("Done").InsertMethod(0, static a =>
        {
            GameObject go = a.fsmComponent.gameObject;
            Action? callback = go.GetComponent<DefaultBigUiDefDataComponent>().Callback;
            callback?.Invoke();
        });
    }
}
