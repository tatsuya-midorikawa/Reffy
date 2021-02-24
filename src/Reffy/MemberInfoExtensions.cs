using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Reffy
{
    public static class MemberInfoExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
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
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static bool TryGetAttribute<T>(this MemberInfo info, out T attribute)
            where T : Attribute
        {
            attribute = info.GetAttribute<T>();
            return attribute != null;
        }
    }
}
