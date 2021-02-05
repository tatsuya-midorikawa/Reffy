using Microsoft.FSharp.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Reffy
{
    public static class UnionCaseInfoExtensions
    {
        public static T GetCustomAttribute<T>(this UnionCaseInfo info)
            where T : Attribute
        {
            if (_unioncaseinfoCache.TryGetValue(info, out Attribute cache) && cache is T)
                return (T)cache;

            if (info.GetCustomAttributes(typeof(T)).FirstOrDefault() is T attribute)
            {
                _unioncaseinfoCache.Add(info, attribute);
                return attribute;
            }
            else
            {
                return null;
            }
        }

        public static bool TryGetCustomAttribute<T>(this UnionCaseInfo info, out T attribute)
            where T : Attribute
        {
            attribute = info.GetCustomAttribute<T>();
            return attribute != null;
        }

        private static Dictionary<UnionCaseInfo, Attribute> _unioncaseinfoCache
            = new Dictionary<UnionCaseInfo, Attribute>();
    }
}
