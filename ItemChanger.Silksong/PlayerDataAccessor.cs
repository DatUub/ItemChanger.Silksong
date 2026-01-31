using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ItemChanger.Silksong
{
    internal static class PlayerDataAccessor
    {
        private static readonly Dictionary<string, Func<PlayerData, bool>> getters = new();
        private static readonly Dictionary<string, Action<PlayerData, bool>> setters = new();

        public static bool GetBool(PlayerData pd, string fieldName)
        {
            if (!getters.TryGetValue(fieldName, out var getter))
            {
                getter = CreateGetter(fieldName);
                getters[fieldName] = getter;
            }
            return getter(pd);
        }

        public static void SetBool(PlayerData pd, string fieldName, bool value)
        {
            if (!setters.TryGetValue(fieldName, out var setter))
            {
                setter = CreateSetter(fieldName);
                setters[fieldName] = setter;
            }
            setter(pd, value);
        }

        private static Func<PlayerData, bool> CreateGetter(string fieldName)
        {
            var pdParam = Expression.Parameter(typeof(PlayerData), "pd");
            var field = typeof(PlayerData).GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);

            Expression body;
            if (field != null && field.FieldType == typeof(bool))
            {
                body = Expression.Field(pdParam, field);
            }
            else
            {
                // Fallback to GetBool(string)
                var method = typeof(PlayerData).GetMethod("GetBool", new[] { typeof(string) });
                if (method == null) throw new MissingMethodException("PlayerData", "GetBool");
                body = Expression.Call(pdParam, method, Expression.Constant(fieldName));
            }

            return Expression.Lambda<Func<PlayerData, bool>>(body, pdParam).Compile();
        }

        private static Action<PlayerData, bool> CreateSetter(string fieldName)
        {
            var pdParam = Expression.Parameter(typeof(PlayerData), "pd");
            var valueParam = Expression.Parameter(typeof(bool), "value");
            var field = typeof(PlayerData).GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);

            Expression body;
            if (field != null && field.FieldType == typeof(bool))
            {
                body = Expression.Assign(Expression.Field(pdParam, field), valueParam);
            }
            else
            {
                // Fallback to SetBool(string, bool)
                var method = typeof(PlayerData).GetMethod("SetBool", new[] { typeof(string), typeof(bool) });
                if (method == null) throw new MissingMethodException("PlayerData", "SetBool");
                body = Expression.Call(pdParam, method, Expression.Constant(fieldName), valueParam);
            }

            return Expression.Lambda<Action<PlayerData, bool>>(body, pdParam, valueParam).Compile();
        }
    }
}
