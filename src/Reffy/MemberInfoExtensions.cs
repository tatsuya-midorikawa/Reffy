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
            if (_memberinfoCache.TryGetValue(info, out Attribute cache) && cache is T)
            {
                attribute = (T)cache;
                return true;
            }

            if ((Attribute.GetCustomAttribute(info, typeof(T)) is T attr))
            {
                _memberinfoCache.Add(info, attr);
                attribute = attr;
                return true;
            }
            else
            {
                attribute = default;
                return false;
            }
        }

        private static Dictionary<MemberInfo, Attribute> _memberinfoCache
            = new Dictionary<MemberInfo, Attribute>();
    }
}
