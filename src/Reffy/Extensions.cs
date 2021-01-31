using System;
using System.Collections.Generic;
using System.Reflection;

namespace Reffy
{
    public static class Extensions
    {
        public static FieldInfo GetBackingField<T>(string propertyName)
        {
            if (_backingfieldCache.TryGetValue(typeof(T), out Dictionary<string, FieldInfo> innerCache))
            {
                if (innerCache.TryGetValue(propertyName, out FieldInfo info))
                    return info;
            }
            else
            {
                _backingfieldCache.Add(typeof(T), new Dictionary<string, FieldInfo>());
                innerCache = _backingfieldCache[typeof(T)];
            }

            innerCache.Add(propertyName, typeof(T).GetField(propertyName.ToBackingFieldName(), BindingFlags.Instance | BindingFlags.NonPublic));
            return innerCache[propertyName];
        }

        public static FieldInfo GetField<T>(string propertyName)
            => typeof(T).GetField(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        public static T GetAttribute<T>(this MemberInfo info)
            where T : Attribute
        {
            if (_memberinfoCache.TryGetValue(info, out Attribute cache) && cache is T)
                return (T)cache;

            if ((Attribute.GetCustomAttribute(info, typeof(T)) is T attribute))
            {
                _memberinfoCache.Add(info, attribute);
                return attribute;
            }
            else
            {
                return null;
            }
        }

        internal static string ToBackingFieldName(this string propertyName)
            => $"<{propertyName}>k__BackingField";

        private static readonly Dictionary<Type, Dictionary<string, FieldInfo>> _backingfieldCache
            = new Dictionary<Type, Dictionary<string, FieldInfo>>();
        private static Dictionary<MemberInfo, Attribute> _memberinfoCache
            = new Dictionary<MemberInfo, Attribute>();
    }
}
