using HutongGames.PlayMaker;
using ItemChanger.Silksong.Extensions;
using ItemChanger.Locations;
using Silksong.FsmUtil;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Silksong.RawData;

namespace ItemChanger.Silksong.Locations;

public class NuuJournalLocation : AutoLocation
{
    protected override void DoLoad()
    {
        Using(new FsmEditGroup
        {
            { new(UnsafeSceneName, "Nuu", "Dialogue"), EditNuu },
        });
    }

    protected override void DoUnload()
    {
    }

    private void EditNuu(PlayMakerFSM fsm)
    {
        FsmState hasJournal = fsm.MustGetState("Has Journal?");
        FsmState journal = fsm.MustGetState("Journal");

        hasJournal.RemoveActionsOfType<PlayerDataVariableTest>();
        hasJournal.AddMethod(() =>
        {
            FullQuestBase quest = QuestManager.GetQuest(Quests.Journal);
            if (!quest.IsAccepted)
            {
                fsm.SendEvent("FALSE");
            }
            else
            {
                this.CreateGiveAllDelegate(fsm, "TRUE").Invoke();
            }
        });

        journal.Actions = [journal.Actions[0]];
        journal.AddMethod(this.CreateGiveAllDelegate(fsm, "GET ITEM MSG END"));
    }
}
