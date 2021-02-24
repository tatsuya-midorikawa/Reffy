using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Mono.Reflection;

namespace Reffy
{
    public static class BackingFieldExtensions
    {
        /// <summary>
        /// プロパティ情報からBackingFieldを取得する.
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
        /// 型情報と取得対象プロパティ名からBackingFieldを取得する.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <param name="flags"></param>
        /// <param name="useCache"></param>
        /// <returns></returns>
        public static FieldInfo GetBackingField(this Type type, string propertyName, BindingFlags flags = (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) ^ BindingFlags.DeclaredOnly, bool useCache = true)
        {
            return type
                .GetProperty(propertyName, flags)
                .GetBackingField(useCache);
        }

        /// <summary>
        /// Type情報からBackingField一覧を取得する.
        /// </summary>
        /// <param name="type">BackingFieldsを取得したいType情報</param>
        /// <param name="useCache">キャッシュ機能を利用する場合はtrueを指定する. default: true</param>
        /// <returns>Backing fields情報配列</returns>
        public static FieldInfo[] GetBackingFields(this Type type, BindingFlags flags = (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) ^ BindingFlags.DeclaredOnly, bool useCache = true)
        {
            if (useCache && _backingfieldsCache.TryGetValue(type.FullName, out FieldInfo[] fields))
                return fields;

            fields = type
                .GetProperties(flags)
                .Select(property => property.GetBackingField())
                .ToArray();

            if (useCache)
                _backingfieldsCache.Add(type.FullName, fields);

            return fields;
        }
        private static Dictionary<string, FieldInfo[]> _backingfieldsCache
            = new Dictionary<string, FieldInfo[]>();
    }
}
