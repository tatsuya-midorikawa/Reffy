using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Reffy
{
    public static class MemberInfoExtensions
    {
        /// <summary>
        /// プロパティ情報からBacking fieldを取得する.
        /// </summary>
        /// <param name="property">Backing fieldsを取得したいプロパティ情報</param>
        /// <param name="useCache">キャッシュ機能を利用する場合はtrueを指定する. default: true</param>
        /// <returns>Backing fields情報</returns>
        public static FieldInfo GetBackingField(this PropertyInfo property, bool useCache = true)
        {
            if (property == null)
                throw new ArgumentNullException("The argument must be a non-null value.");

            if (useCache && _backingfieldCache.TryGetValue(property, out FieldInfo field))
                return field;

            field = BackingFieldResolver.GetBackingField(property);
            if (useCache)
                _backingfieldCache.Add(property, field);
            return field;
        }
        private static readonly Dictionary<PropertyInfo, FieldInfo> _backingfieldCache
            = new Dictionary<PropertyInfo, FieldInfo>();

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
