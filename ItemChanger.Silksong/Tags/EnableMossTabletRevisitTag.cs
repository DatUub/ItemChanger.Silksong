using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Locations;
using ItemChanger.Silksong.FsmStateActions;
using ItemChanger.Tags;
using ItemChanger.Tags.Constraints;
using Newtonsoft.Json;
using Silksong.FsmUtil;

namespace ItemChanger.Silksong.Tags;

/*
 * The vanilla behaviour is:
 * - The non-needolin tablet is checkable iff it has never been blown off by the needolin
 * - The needolin tablet is checkable iff the needolin has been played since the scene was entered
 */

/// <summary>
/// Tag used to allow revisiting the non-needolin tablet below silkspear.
/// </summary>
[LocationTag]
public class EnableMossTabletRevisitTag : Tag
{
    [JsonIgnore]
    private Location? _parent;

    protected override void DoLoad(TaggableObject parent)
    {
        _parent = (parent as Location)!;
        Using(new FsmEditGroup()
        {
            { new FsmId(_parent.SceneName!, "moss_bone_plaque", "Control"), EnableRevisit }
        });
    }

    protected override void DoUnload(TaggableObject parent)
    {
        _parent = null;
    }

    private void EnableRevisit(PlayMakerFSM fsm)
    {
        FsmState state = fsm.MustGetState("Init");
        state.ReplaceActionsOfType<BoolTest>(orig => new CustomCheckFsmStateAction(orig) { GetIsTrue = () => _parent!.Placement!.AllObtained() });
    }
}
