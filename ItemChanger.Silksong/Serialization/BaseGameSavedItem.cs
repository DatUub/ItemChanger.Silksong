using ItemChanger.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace ItemChanger.Silksong.Serialization;

// Regarding the hierarchy of base game types:

// QuestTargetCounter inherits from SavedItem and implements ICollectableUIMsgItem, IUIMsgPopupItem
// CollectableItem inherits from QuestTargetCounter and implements ICollectionViewerItem
// CollectableRelic inherits from QuestTargetCounter and implements ICollectionViewerItem
// ToolCrest, ToolItem inherit from ToolBase, which inherits from QuestTargetCounter.
// MateriumItem inherits from SavedItem and implements ICollectionViewerItem
// EnemyJournalRecord inherits from QuestTargetCounter (with the effect of giving 1 kill).

// SavedItem is the common base class of all of these types, providing Get, GetPopupName, GetPopupIcon, CanGetMore, among other things.
// However, the virtual implementations of GetPopupName, GetPopupIcon throw exceptions, and not all subclasses override them.

// Descriptions are not part of SavedItem. They are provided through ICollectionViewerItem in most cases.
// For tools, they are provided through ToolCrest.Description or ToolItem.Description, which are not inherited or part of any interface.

/// <summary>
/// A wrapper to manage finding base game items from CollectableItemManager, CollectableRelicManager, ToolItemManager, MateriumItemManager, or EnemyJournalManager.
/// </summary>
public class BaseGameSavedItem : IValueProvider<SavedItem>
{
    /// <summary>
    /// The name of the SavedItem, as a UnityEngine.Object.
    /// </summary>
    public required string Id { get; init; }
    /// <summary>
    /// The derived type of SavedItem, which also dictates where the item should be found.
    /// </summary>
    public required ItemType Type { get; init; }

    public enum ItemType
    {
        CollectableItem,
        CollectableRelic,
        ToolCrest,
        ToolItem,
        MateriumItem,
        EnemyJournalRecord
    }

    [JsonIgnore]
    public SavedItem Value { get => field ? field : (field = FindItem()); }

    private SavedItem FindItem()
    {
        return Type switch
        {
            ItemType.CollectableItem => CollectableItemManager.GetItemByName(Id),
            ItemType.CollectableRelic => CollectableRelicManager.GetRelic(Id),
            ItemType.ToolCrest => ToolItemManager.GetCrestByName(Id),
            ItemType.ToolItem => ToolItemManager.GetToolByName(Id),
            ItemType.MateriumItem => MateriumItemManager.Instance.MasterList.GetByName(Id),
            ItemType.EnemyJournalRecord => EnemyJournalManager.Instance.recordList.GetByName(Id),
            _ => throw new NotImplementedException(Type.ToString()),
        };
    }

    /// <summary>
    /// Gives the effect of the item, without a UI message.
    /// For some items, such as silk skills, additional bools are ordinarily set by fsm, so this does not always give the full effect of the item.
    /// </summary>
    public void Get() => Value.Get(showPopup: false);
    /// <summary>
    /// For an item with a unique PD bool, returns true if the bool has not been set.
    /// For an item with a capped counter, returns true if the counter value is below the cap.
    /// </summary>
    public bool CanGetMore() => Value.CanGetMore();
    /// <summary>
    /// Returns the localized string for the name of the item as it appears in a UI popup.
    /// </summary>
    public string GetCollectionName() => Value switch
    {
        CollectableItem or CollectableRelic or ToolItem or MateriumItem => Value.GetPopupName(),
        ToolCrest tc => (string)tc.DisplayName,
        EnemyJournalRecord ejr => (string)ejr.DisplayName,
        _ => throw new NotImplementedException(Type.ToString()),
    };
    /// <summary>
    /// Returns the localized description for the item.
    /// </summary>
    public string GetCollectionDesc() => Value switch
    {
        ICollectionViewerItem view => view.GetCollectionDesc(), // covers CollectableItem, CollectableRelic, MateriumItem
        ToolCrest tc => (string)tc.Description,
        ToolItem ti => (string)ti.Description,
        EnemyJournalRecord ejr => (string)ejr.Description,
        _ => throw new NotImplementedException(Value.GetType().Name),
    };
    /// <summary>
    /// Returns the sprite for the item as it appears in a UI popup.
    /// </summary>
    public Sprite GetCollectionSprite() => Value switch
    {
        CollectableItem or CollectableRelic or MateriumItem or EnemyJournalRecord or ToolItem or ToolCrest => Value.GetPopupIcon(),
        _ => throw new NotImplementedException(Type.ToString()),
    };
}
