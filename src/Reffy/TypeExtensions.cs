using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Reflection;
using Mono.Reflection;

namespace Reffy
{
    public static class TypeExtensions
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
        /// Type情報からBacking field一覧を取得する.
        /// </summary>
        /// <param name="type">Backing fieldsを取得したいType情報</param>
        /// <param name="useCache">キャッシュ機能を利用する場合はtrueを指定する. default: true</param>
        /// <returns>Backing fields情報配列</returns>
        public static FieldInfo[] GetBackingFields(this Type type, bool useCache = true)
        {
            if (useCache && _backingfieldsCache.TryGetValue(type.FullName, out FieldInfo[] fields))
                return fields;

            fields = type
                .GetProperties((BindingFlags.Instance | BindingFlags.NonPublic) ^ BindingFlags.DeclaredOnly)
                .Select(property => property.GetBackingField())
                .ToArray();

            if (useCache)
                _backingfieldsCache.Add(type.FullName, fields);

            return fields;
        }
        private static Dictionary<string, FieldInfo[]> _backingfieldsCache
            = new Dictionary<string, FieldInfo[]>();

        /// <summary>
        /// Type情報からデフォルトのインスタンスを生成する.
        /// </summary>
        /// <param name="type">デフォルトインスタンスを生成したいType情報</param>
        /// <returns>Type情報から生成したインスタンス</returns>
        public static object MakeDefault(this Type type)
        {
            var none = FSharpOption<BindingFlags>.None;

            // 文字列型
            if (type == typeof(string))
            {
                return null;
            }

            // Option型(F#)
            //   - FSharpType.IsUnion()でも引っ掛かってしまうので、
            //     それよりも前に判定をする必要がある.
            if (type.IsGenericType
                && !type.IsGenericTypeDefinition
                && !type.IsGenericParameter
                && typeof(FSharpOption<>) == type.GetGenericTypeDefinition())
            {
                return type
                    .GetProperty("None", BindingFlags.Public | BindingFlags.Static)
                    .GetGetMethod()
                    .Invoke(null, null);
            }

            // 判別共用体(F#)
            if (FSharpType.IsUnion(type, none))
            {
                var ctor = FSharpType.GetUnionCases(type, none).FirstOrDefault();
                if (ctor == null)
                    throw new Exception("Invalid discriminated-unions.");
                var ps = ctor.GetFields();
                var qs = new object[ps.Length];
                for (var i = 0; i < ps.Length; i++)
                    qs[i] = ps[i].PropertyType.MakeDefault();

                return FSharpValue.MakeUnion(ctor, qs, none);
            }

            // 列挙型
            if (type.IsEnum)
            {
                return type.GetEnumValue(0);
            }

            // 構造体
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            // クラス
            if (type.IsClass)
            {
                var ctor = type.GetConstructors().FirstOrDefault();

                // publicコンストラクタが存在しない場合、nullを返す
                if (ctor is null)
                {
                    return null;
                }
                else
                {
                    var ps = ctor.GetParameters();
                    var qs = new object[ps.Length];
                    for (int i = 0; i < ps.Length; i++)
                        qs[i] = ps[i].ParameterType.MakeDefault();

                    return Activator.CreateInstance(type, qs);
                }
            }

            throw new ArgumentException("'type' is not supported.");
        }
    }
}
