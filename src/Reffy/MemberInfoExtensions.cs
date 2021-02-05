using System;
using System.Collections.Generic;
using System.Reflection;

namespace Reffy
{
    public static class MemberInfoExtensions
    {
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

        public static bool TryGetAttribute<T>(this MemberInfo info, out T attribute)
            where T : Attribute
        {
            attribute = info.GetAttribute<T>();
            return attribute != null;
        }

        private static Dictionary<MemberInfo, Attribute> _memberinfoCache
            = new Dictionary<MemberInfo, Attribute>();
    }
}
