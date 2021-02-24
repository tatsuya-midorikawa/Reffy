#if NET40 || NET45 || NET46 || NET472 || NET48 || NETCOREAPP3_1 || NET5_0
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using Microsoft.FSharp.Reflection;
#if NET5_0 || NETCOREAPP3_1
using System.Diagnostics.CodeAnalysis;
#endif

namespace Reffy.Expressions
{
    public static class ConstructorExtensions
    {
        private static Func<object[], object> BuildConstructor(Type type, Type[] paramsTypes, object[] @params)
        {
            var ctorinfo = type.GetConstructor(paramsTypes);
            if (ctorinfo == null)
                throw new ArgumentException("No costractor with matching arguments.");

            var args = Expression.Parameter(typeof(object[]), "args");
            var paramsExpr = @params
                .Select((x, i) =>
                    Expression.Convert(
                        Expression.ArrayIndex(args, Expression.Constant(i)),
                        x.GetType()))
                .ToArray();

            var ctor = Expression.Lambda<Func<object[], object>>(
                Expression.Convert(
                    Expression.New(ctorinfo, paramsExpr),
                    typeof(object)),
                args).Compile();

            return ctor;
        }

        /// <summary>
        /// コンストラクタ呼び出し.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="params"></param>
        /// <returns></returns>
#if NET5_0 || NETCOREAPP3_1
        public static object Constructor<T>([DisallowNull] this Type type, [AllowNull] params object[] @params)
#else
        public static T Constructor<T>(this Type type, params object[] @params)
#endif
        {
            return Constructor(type, @params) is T instance
                ? instance
                : throw new Exception($"Created instance type is not {typeof(T).FullName}.");
        }

        /// <summary>
        /// コンストラクタ呼び出し.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="params"></param>
        /// <returns></returns>
#if NET5_0 || NETCOREAPP3_1
        public static object Constructor([DisallowNull] this Type type, [AllowNull] params object[] @params)
#else
        public static object Constructor(this Type type, params object[] @params)
#endif
        {
            @params = @params == null ? Type.EmptyTypes : @params;
            var types = new RuntimeTypeHandle[@params.Length + 1];
            types[0] = type.TypeHandle;
            for (int i = 0; i < @params.Length; i++)
                types[i + 1] = Type.GetTypeHandle(@params[i]);

            if (_constructorCache.TryGetValue(types, out Func<object[], object> ctor))
                return ctor(@params);
            // slow path.
            var types2 = new Type[@params.Length];
            for (int i = 0; i < @params.Length; i++)
                types2[i] = Type.GetTypeFromHandle(types[i + 1]);
            ctor = BuildConstructor(type, types2, @params);
            return _constructorCache.TryAdd(types, ctor)(@params);
        }
        private static readonly Cache _constructorCache = new Cache();

        private class Cache
        {
            private ConcurrentDictionary<RuntimeTypeHandle[], Func<object[], object>> _cache
                = new ConcurrentDictionary<RuntimeTypeHandle[], Func<object[], object>>(new TypeArrayEquallity());

#if NET5_0 || NETCOREAPP3_1
            public bool TryGetValue([DisallowNull] RuntimeTypeHandle[] types, out Func<object[], object> constructor)
#else
            public bool TryGetValue(RuntimeTypeHandle[] types, out Func<object[], object> constructor)
#endif
                => _cache.TryGetValue(types, out constructor);

#if NET5_0 || NETCOREAPP3_1
            public Func<object[], object> TryAdd([DisallowNull] RuntimeTypeHandle[] types, [DisallowNull] Func<object[], object> constructor)
#else
            public Func<object[], object> TryAdd(RuntimeTypeHandle[] types, Func<object[], object> constructor)
#endif
            {
                _cache.TryAdd(types, constructor);
                return constructor;
            }

            private class TypeArrayEquallity : IEqualityComparer<RuntimeTypeHandle[]>
            {
#if NET5_0 || NETCOREAPP3_1
                public bool Equals([AllowNull] RuntimeTypeHandle[] x, [AllowNull] RuntimeTypeHandle[] y)
#else
                public bool Equals(RuntimeTypeHandle[] x, RuntimeTypeHandle[] y)
#endif
                {
                    if (x.Length != y.Length) return false;
                    for (int i = 0; i < x.Length; ++i)
                        if (x[i].Value != y[i].Value) return false;
                    return true;
                }

#if NET5_0 || NETCOREAPP3_1
                public int GetHashCode([DisallowNull] RuntimeTypeHandle[] obj)
#else
                public int GetHashCode(RuntimeTypeHandle[] obj)
#endif
                {
                    int returnValue = obj[0].GetHashCode();
                    for (int i = 1; i < obj.Length; ++i)
                        returnValue ^= obj[i].GetHashCode();
                    return returnValue;
                }
            }
        }
    }
}
#endif
