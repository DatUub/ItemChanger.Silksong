using ItemChanger.Serialization;
using ItemChanger.Silksong;
using Newtonsoft.Json;

namespace ItemChanger.Silksong.Serialization
{
    public record PDBool(string BoolName) : IBool, IWritableBool
    {
        [JsonIgnore]
        public bool Value
        {
            get
            {
                return PlayerDataAccessor.GetBool(PlayerData.instance, BoolName);
            }
            set
            {
                PlayerDataAccessor.SetBool(PlayerData.instance, BoolName, value);
            }
        }

        IBool IBool.Clone()
        {
            return this with { };
        }
    }
}
