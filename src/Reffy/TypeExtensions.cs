using System;
using System.Linq;
using System.Reflection;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Reflection;

namespace Reffy
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Type情報からデフォルトのインスタンスを生成する
        /// </summary>
        /// <param name="type">デフォルトインスタンスを生成したいType情報</param>
        /// <returns>Type情報から生成したインスタンス</returns>
        public static object CreateDefaultInstance(this Type type)
        {
            // 判別共用体(F#)
            if (FSharpType.IsUnion(type, FSharpOption<BindingFlags>.None))
            {
                var ctor = FSharpType.GetUnionCases(type, FSharpOption<BindingFlags>.None).FirstOrDefault();
                if (ctor == null)
                    throw new Exception("Invalid type.");
                var ps = ctor.GetFields();
                var qs = new object[ps.Length];
                for (var i = 0; i < ps.Length; i++)
                {
                    qs[i] = ps[i].PropertyType.IsValueType
                        ? Activator.CreateInstance(ps[i].PropertyType)
                        : null;
                }

                return FSharpValue.MakeUnion(ctor, qs, FSharpOption<BindingFlags>.None);
            }

            // 列挙型
            if (type.IsEnum)
                return Enum.GetValues(type).GetValue(0);

            // 構造体
            if (type.IsValueType)
                return Activator.CreateInstance(type);

            // クラス
            if (type.IsClass)
            {
                var ctor = type.GetConstructors().FirstOrDefault();
                if (ctor is null)
                    throw new Exception("Public constructor does not exist.");

                var ps = ctor.GetParameters();
                var qs = new object[ps.Length];
                for (int i = 0; i < ps.Length; i++)
                {
                    qs[i] = ps[i].ParameterType.IsValueType
                        ? Activator.CreateInstance(ps[i].ParameterType)
                        : null;
                }
                return Activator.CreateInstance(type, qs);
            }

            throw new ArgumentException("'type' is not supported.");
        }
    }
}
