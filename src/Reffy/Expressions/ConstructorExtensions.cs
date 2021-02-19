#if NET40 || NET45 || NET46 || NET472 || NET48 || NETCOREAPP3_1 || NET5_0
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using Microsoft.FSharp.Reflection;

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
        /// 呼び出し型情報と引数の型情報でコンストラクタをキャッシュします. 
        /// <see cref="RestrictedConstructor(Type, object[])"/>よりも低速ですが、初回に呼び出したコンストラクタと同一個数の引数をもつ別のコンストラクタを呼び出しても正常に動作します.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public static object Constructor(this Type type, params object[] @params)
        {
            @params = @params == null ? Type.EmptyTypes : @params;
            var types = new Type[@params.Length];
            for (int i = 0; i < types.Length; i++)
                types[i] = @params[i].GetType();

            var key = string.Concat(types as object[]);
            if (_constructorCache.TryGetValue(type, types, out Func<object[], object> ctor))
                return ctor(@params);

            ctor = BuildConstructor(type, types, @params);
            return _constructorCache.GetOrAdd(type, types, ctor)(@params);
        }
        private static readonly Cache _constructorCache = new Cache();

        private class Cache
        {
            private ConcurrentDictionary<Type, List<Data>> _cache
                = new ConcurrentDictionary<Type, List<Data>>();

            public bool TryGetValue(Type type, Type[] types, out Func<object[], object> contructor)
            {
                contructor = null;
                if (_cache.TryGetValue(type, out List<Data> cache))
                {
                    foreach (var ts in cache)
                    {
                        if (ts.ArgTypes.Length != types.Length)
                            continue;

                        var same = true;
                        for (int i = 0; i < ts.ArgTypes.Length; i++)
                        {
                            if (ts.ArgTypes[i] != types[i])
                            {
                                same = false;
                                break;
                            }
                        }

                        if (!same)
                            continue;

                        contructor = ts.Constructor;
                        return same;
                    }
                }
                return false;
            }

            public Func<object[], object> GetOrAdd(Type type, Type[] types, Func<object[], object> contructor)
            {
                if (!_cache.TryGetValue(type, out List<Data> cache))
                {
                    cache = new List<Data>();
                    _cache.TryAdd(type, cache);
                }

                if (TryGetValue(type, types, out Func<object[], object>  ctor))
                {
                    return ctor;
                }

                cache.Add(new Data(types, contructor));
                return contructor;
            }

            public class Data
            {
                public Type[] ArgTypes { get; }
                public Func<object[], object> Constructor { get; }

                public Data(Type[] argTypes, Func<object[], object> constructor)
                {
                    ArgTypes = argTypes;
                    Constructor = constructor;
                }
            }
        }

        /// <summary>
        /// 限定的なコンストラクタ呼び出し.
        /// 引数の個数でコンストラクタ情報をキャッシュするので、初回に呼び出したコンストラクタと同一個数の引数をもつ別のコンストラクタを呼び出すとエラーとなります.
        /// その代わり<see cref="Constructor(Type, object[])"/>に比べて高速に動作します。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public static object RestrictedConstructor(this Type type, params object[] @params)
        {
            var key = @params?.Length ?? 0;
            if (_restrictedConstructorCache.TryGetValue(type, out List<KeyValuePair<int, Func<object[], object>>> innerCache))
            {
                // inner cacheの中にコンストラクタのキャッシュ情報があればそれを利用する
                foreach (var cache in innerCache)
                {
                    if (cache.Key == key)
                        return cache.Value(@params);
                }
            }
            else
            {
                // _constructorCacheの中にinner cacheが存在しないときのみ、inner cacheを追加する
                innerCache = new List<KeyValuePair<int, Func<object[], object>>>();
                _restrictedConstructorCache.TryAdd(type, innerCache);
            }

            var types = @params == null ? Type.EmptyTypes : @params.Select(p => p.GetType()).ToArray();
            var ctor = BuildConstructor(type, types, @params);
            innerCache.Add(new KeyValuePair<int, Func<object[], object>>(key, ctor));
            return ctor(@params);
        }
        private static readonly ConcurrentDictionary<Type, List<KeyValuePair<int, Func<object[], object>>>> _restrictedConstructorCache
            = new ConcurrentDictionary<Type, List<KeyValuePair<int, Func<object[], object>>>>();

    }
}
#endif
