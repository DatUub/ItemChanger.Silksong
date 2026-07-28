using ItemChanger.Extensions;
using ItemChanger.Serialization;
using System.Reflection;
using UnityEngine.SceneManagement;

namespace ItemChanger.Silksong.Serialization
{
    /// <summary>
    /// Value provider which fetches a struct field from a component on a GameObject in an active scene, in a Nullable wrapper.
    /// If the specified scene is not active, or the object/component are not found, outputs null.
    /// </summary>
    public record ComponentFieldOption<TComponent, TField>(string SceneName, string ObjectPath, string FieldName) 
        : IValueProvider<TField?> where TField : struct
    {
        private readonly FieldInfo fi = typeof(TComponent)
            .GetField(FieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            ?? throw new ArgumentException($"Field {FieldName} of {typeof(TField)} was not found.");

        public TField? Value
        {
            get
            {
                Scene scene = SceneManager.GetSceneByName(SceneName);
                if (!scene.IsValid()) return null;
                GameObject? go = scene.FindGameObject(ObjectPath);
                if (go == null) return null;
                TComponent obj = go.GetComponent<TComponent>();
                if (obj == null) return null;
                return (TField)fi.GetValue(obj);
            }
        }
    }
}
