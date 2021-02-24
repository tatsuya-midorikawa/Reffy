using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Reflection;
using Mono.Reflection;
#if NET40 || NET45 || NET46 || NET472 || NET48 || NETCOREAPP3_1 || NET5_0
using Reffy.Expressions;
#endif

namespace Reffy
{
    public static class TypeExtensions
    {
        /// <summary>
        /// F#のOption型かの判定
        /// </summary>
        /// <param name="type">判定対象の型情報</param>
        /// <returns>F# Option型の場合 true</returns>
        public static bool IsFsharpOption(this Type type)
        {
            return type.IsGenericType
                && !type.IsGenericTypeDefinition
                && !type.IsGenericParameter
                && typeof(FSharpOption<>) == type.GetGenericTypeDefinition();
        }

        /// <summary>
        /// F#の判別共用体型かの判定
        /// </summary>
        /// <param name="type">判定対象の型情報</param>
        /// <returns>F# 判別共用体の場合 true</returns>
        public static bool IsFsharpDiscriminatedUnions(this Type type)
        {
            return !type.IsFsharpOption() && FSharpType.IsUnion(type, FSharpOption<BindingFlags>.None);
        }

        /// <summary>
        /// Type情報からデフォルトのインスタンスを生成する.
        /// </summary>
        /// <param name="type">デフォルトインスタンスを生成したいType情報</param>
        /// <returns>Type情報から生成したインスタンス</returns>
        public static object MakeDefault(this Type type)
        {
            var none = FSharpOption<BindingFlags>.None;

            if (type == typeof(int))
                return default(int);
            if (type == typeof(uint))
                return default(uint);
            if (type == typeof(short))
                return default(short);
            if (type == typeof(ushort))
                return default(ushort);
            if (type == typeof(long))
                return default(long);
            if (type == typeof(ulong))
                return default(ulong);
            if (type == typeof(byte))
                return default(byte);
            if (type == typeof(sbyte))
                return default(sbyte);
            if (type == typeof(bool))
                return default(bool);
            if (type == typeof(float))
                return default(float);
            if (type == typeof(double))
                return default(double);
            if (type == typeof(decimal))
                return default(decimal);
            if (type == typeof(char))
                return default(char);
            if (type == typeof(string))
                return default(string);
            if (type == typeof(Guid))
                return default(Guid);
            if (type == typeof(DateTime))
                return default(DateTime);
            if (type == typeof(DateTimeOffset))
                return default(DateTimeOffset);
            if (type == typeof(byte[]))
                return default(byte[]);

            if (type == typeof(int?))
                return default(int?);
            if (type == typeof(uint?))
                return default(uint?);
            if (type == typeof(short?))
                return default(short?);
            if (type == typeof(ushort?))
                return default(ushort?);
            if (type == typeof(long?))
                return default(long?);
            if (type == typeof(ulong?))
                return default(ulong?);
            if (type == typeof(byte?))
                return default(byte?);
            if (type == typeof(sbyte?))
                return default(sbyte?);
            if (type == typeof(bool?))
                return default(bool?);
            if (type == typeof(float?))
                return default(float?);
            if (type == typeof(double?))
                return default(double?);
            if (type == typeof(decimal?))
                return default(decimal?);
            if (type == typeof(char?))
                return default(char?);
            if (type == typeof(Guid?))
                return default(Guid?);
            if (type == typeof(DateTime?))
                return default(DateTime?);
            if (type == typeof(DateTimeOffset?))
                return default(DateTimeOffset?);

            if (type == typeof(FSharpOption<int>))
                return FSharpOption<int>.None;
            if (type == typeof(FSharpOption<uint>))
                return FSharpOption<uint>.None;
            if (type == typeof(FSharpOption<short>))
                return FSharpOption<short>.None;
            if (type == typeof(FSharpOption<ushort>))
                return FSharpOption<ushort>.None;
            if (type == typeof(FSharpOption<long>))
                return FSharpOption<long>.None;
            if (type == typeof(FSharpOption<ulong>))
                return FSharpOption<ulong>.None;
            if (type == typeof(FSharpOption<byte>))
                return FSharpOption<byte>.None;
            if (type == typeof(FSharpOption<sbyte>))
                return FSharpOption<sbyte>.None;
            if (type == typeof(FSharpOption<bool>))
                return FSharpOption<bool>.None;
            if (type == typeof(FSharpOption<float>))
                return FSharpOption<float>.None;
            if (type == typeof(FSharpOption<double>))
                return FSharpOption<double>.None;
            if (type == typeof(FSharpOption<decimal>))
                return FSharpOption<decimal>.None;
            if (type == typeof(FSharpOption<char>))
                return FSharpOption<char>.None;
            if (type == typeof(FSharpOption<string>))
                return FSharpOption<string>.None;
            if (type == typeof(FSharpOption<Guid>))
                return FSharpOption<Guid>.None;
            if (type == typeof(FSharpOption<DateTime>))
                return FSharpOption<DateTime>.None;
            if (type == typeof(FSharpOption<DateTimeOffset>))
                return FSharpOption<DateTimeOffset>.None;
            if (type == typeof(FSharpOption<byte[]>))
                return FSharpOption<byte[]>.None;

            // Option型(F#)
            //   - FSharpType.IsUnion()でも引っ掛かってしまうので、
            //     それよりも前に判定をする必要がある.
            if (type.IsFsharpOption())
            {
                return type
                    .GetProperty("None", BindingFlags.Public | BindingFlags.Static)
                    .GetGetMethod()
                    .Invoke(null, null);
            }

            // 判別共用体(F#)
            if (type.IsFsharpDiscriminatedUnions())
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

#if NET40 || NET45 || NET46 || NET472 || NET48 || NETCOREAPP3_1 || NET5_0
                    return type.Constructor(qs);
#else
                    return Activator.CreateInstance(type, qs);
#endif
                }
            }

            throw new ArgumentException("'type' is not supported.");
        }
    }
}
