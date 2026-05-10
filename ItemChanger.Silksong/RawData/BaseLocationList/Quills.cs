using Benchwarp.Data;
using ItemChanger.Locations;
using ItemChanger.Silksong.Locations;

namespace ItemChanger.Silksong.RawData;

internal static partial class BaseLocationList
{
    public static Location Quill__Red => new TrobbioDeskLocation()
    {
        Name = LocationNames.Quill__Red,
        SceneName = SceneNames.Library_13b,
        Act = 2,
    };

    public static Location Quill__Purple => new TrobbioDeskLocation()
    {
        Name = LocationNames.Quill__Purple,
        SceneName = SceneNames.Library_13b,
        Act = 3,
    };
}
