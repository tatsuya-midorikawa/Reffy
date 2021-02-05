using Microsoft.FSharp.Core;
using Microsoft.FSharp.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Reffy
{
    public static class StringExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static FieldInfo GetBackingField<T>(this string propertyName)
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

            var backingfiledName = propertyName.ToBackingFieldName(FSharpType.IsUnion(typeof(T), FSharpOption<BindingFlags>.None));
            innerCache.Add(propertyName, typeof(T).GetField(backingfiledName, BindingFlags.Instance | BindingFlags.NonPublic));
            return innerCache[propertyName];
        }

        private static readonly Dictionary<Type, Dictionary<string, FieldInfo>> _backingfieldCache
            = new Dictionary<Type, Dictionary<string, FieldInfo>>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static FieldInfo GetField<T>(this string fieldName)
            => typeof(T).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        /// <summary>
        /// プロパティ名からBacking field名を生成する
        /// </summary>
        /// <param name="propertyName">対象のプロパティ名</param>
        /// <param name="isUnion">判別共用体(F#)の場合、trueを指定</param>
        /// <returns>Backing field名</returns>
        internal static string ToBackingFieldName(this string propertyName, bool isUnion = false)
        {
            // 判別共用体(F#)の場合、プロパティ名によってBacking field名が変化するので特殊対応する
            //      PascalCase -> camelCase
            //      camelCase -> _camelCase
            //      日本語 -> _日本語
            if (isUnion)
            {
                var c = propertyName[0];
                if ('A' <= c && c <= 'Z')
                    return (char)(c + 32) + propertyName.Substring(1);
                if ('a' <= c && c <= 'z')
                    return "_" + propertyName;

                return "_" + propertyName;
            }
            // 判別共用体(F#)以外の場合は、定型のBacking field名を生成する
            else
            {
                return $"<{propertyName}>k__BackingField";
            }
        }
    }
}
