using System;
using System.Collections.Generic;
#if NET40 || NET45 || NET46 || NET472 || NET48 || NETCOREAPP3_1 || NET5_0
using System.Collections.Concurrent;
#endif

namespace Reffy.Caches
{
    public static class SimpleCache<T, U>
    {
#if NET40 || NET45 || NET46 || NET472 || NET48 || NETCOREAPP3_1 || NET5_0
        private static ConcurrentDictionary<T, U> _cache = new ConcurrentDictionary<T, U>();

        public static bool TryGetValue(T key, out U value)
            => _cache.TryGetValue(key, out value);

        public static bool TryAdd(T key, U value)
            => _cache.TryAdd(key, value);

        public static U GetOrAdd(T key, U value)
            => _cache.GetOrAdd(key, value);
#else
        private static Dictionary<T, U> _cache = new Dictionary<T, U>();

        public static bool TryGetValue(T key, out U value)
            => _cache.TryGetValue(key, out value);

        public static bool TryAdd(T key, U value)
        {
            if (_cache.TryGetValue(key, out U _))
                return false;

            lock (_cache)
            {
                _cache.Add(key, value);
            }
            return true;
        }

        public static U GetOrAdd(T key, U value)
        {
            if (_cache.TryGetValue(key, out U v))
                return v;

            if (TryAdd(key, value))
                return value;

            throw new Exception("'value' add failed.");
        }
#endif
    }
}
