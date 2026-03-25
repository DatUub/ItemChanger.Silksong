using HutongGames.PlayMaker.Actions;
using ItemChanger.Containers;
using ItemChanger.Enums;
using ItemChanger.Items;
using UnityEngine;

namespace ItemChanger.Silksong.Containers
{
    /// <summary>
    /// A <see cref="SavedItem"/> implemented to be compatible with ItemChanger containers.
    /// </summary>
    public class SavedContainerItem : SavedItem
    {
        public required ContainerInfo ContainerInfo { get; set; }
        public MessageType SupportedMessageTypes { get; set; } = MessageType.Any;
        public required Transform ContainerTransform { get; set; }
        public Action? Callback { get; set; } = null;

        // refer to CheckActivation() and activatedRead field of CollectableItemPickup.
        public override bool IsUnique => true;
        public override bool CanGetMore() => ContainerInfo.GiveInfo.Items.Any(i => !i.IsObtained());

        public override void Get(bool showPopup = true)
        {
            // If the container supports large popups, we should have the item keep control
            // while it is giving items; large popups should not take control themselves.
            // If not, we should not take control at all.
            Action? callback = Callback;
            if (SupportedMessageTypes.HasFlag(MessageType.LargePopup))
            {
                GameObject controlKeeper = new("ItemChanger Control Keeper");
                UObject.DontDestroyOnLoad(controlKeeper);
                UIMsgProxy proxy = controlKeeper.AddComponent<UIMsgProxy>();
                proxy.SetIsInMsg(true);

                callback += () =>
                {
                    proxy.SetIsInMsg(false);
                    UObject.Destroy(controlKeeper);
                };
            }

            ContainerInfo.GiveInfo.Placement.GiveSome(ContainerInfo.GiveInfo.Items, new GiveInfo()
            {
                Container = ContainerInfo.ContainerType,
                FlingType = ContainerInfo.GiveInfo.FlingType,
                MessageType = SupportedMessageTypes,
                Transform = ContainerTransform,
            }, callback);
        }
    }

}
