using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Silksong.Components;
using ItemChanger.Silksong.Serialization;
using Silksong.FsmUtil;

namespace ItemChanger.Silksong.Extensions;

public static class PlayMakerFSMExtensions
{
    extension(GameObject obj)
    {
        /// <summary>
        /// If the object is active, immediately edits its fsm. If it is inactive, adds an <see cref="EditFsmOnEnable"/> to edit the fsm once it is activated.
        /// </summary>
        public void EditFsm(string fsmName, Action<PlayMakerFSM> edit)
        {
            if (obj.activeInHierarchy)
            {
                edit(obj.LocateMyFSM(fsmName));
            }
            else
            {
                EditFsmOnEnable editor = obj.AddComponent<EditFsmOnEnable>();
                editor.FsmName = fsmName;
                editor.Edit = edit;
            }
        }
    }

    extension(FsmState state)
    {
        public void AddDynamicDialogueActions(Func<string> dialogGenerator)
        {
            Guid id = Guid.NewGuid();
            FsmString str = state.Fsm.AddStringVariable(id.ToString());
            state.AddMethod(() => str.Value = dialogGenerator());
            state.AddAction(new RunDialogueV2
            {
                CustomText = str,
                PlayerVoiceTableOverride = new() { Value = null },
                PreventHeroAnimation = false,
                HideDecorators = false,
                TextAlignment = TMProOld.TextAlignmentOptions.TopLeft,
                OffsetY = 0f,
                OverrideContinue = false,
                Target = new() { OwnerOption = OwnerDefaultOption.UseOwner },
            });
        }

        public void AddRunDialogueAction(LanguageString str)
        {
            state.AddAction(new RunDialogue
            {
                Sheet = str.Sheet,
                Key = str.Key,
                PlayerVoiceTableOverride = new() { Value = null },
                PreventHeroAnimation = false,
                HideDecorators = false,
                TextAlignment = TMProOld.TextAlignmentOptions.TopLeft,
                OffsetY = 0f,
                OverrideContinue = false,
                Target = new() { OwnerOption = OwnerDefaultOption.UseOwner },
            });
        }
    }
}
