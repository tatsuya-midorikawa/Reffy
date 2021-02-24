using Microsoft.FSharp.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Reffy
{
    public static class AttributeExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        [Obsolete]
        public static T GetAttribute<T>(this MemberInfo info)
            where T : Attribute
        {
            return Attribute.GetCustomAttribute(info, typeof(T)) is T attribute
                ? attribute
                : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        public static T GetCustomAttribute<T>(this MemberInfo info)
            where T : Attribute
        {
            return Attribute.GetCustomAttribute(info, typeof(T)) is T attribute
                ? attribute
                : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        [Obsolete]
        public static bool TryGetAttribute<T>(this MemberInfo info, out T attribute)
            where T : Attribute
        {
            attribute = info.GetCustomAttribute<T>();
            return attribute != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static bool TryGetCustomAttribute<T>(this MemberInfo info, out T attribute)
            where T : Attribute
        {
            attribute = info.GetCustomAttribute<T>();
            return attribute != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
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
