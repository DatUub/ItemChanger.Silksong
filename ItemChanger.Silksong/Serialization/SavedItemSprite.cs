using ItemChanger.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace ItemChanger.Silksong.Serialization
{
    /// <summary>
    /// A wrapper to retrieve a sprite from a SavedItem, located by <see cref="BaseGameSavedItem"/>.
    /// </summary>
    public class SavedItemSprite : IValueProvider<Sprite>
    {
        /// <summary>
        /// Stores the information to locate the item holding the sprite at runtime.
        /// </summary>
        public required BaseGameSavedItem Item { get; init; }

        [JsonIgnore]
        public Sprite Value => Item.GetCollectionSprite();
    }
}
