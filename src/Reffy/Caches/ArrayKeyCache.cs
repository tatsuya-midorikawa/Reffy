using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
#if NET40 || NET45 || NET46 || NET472 || NET48 || NETCOREAPP3_1 || NET5_0
using System.Collections.Concurrent;
#endif

namespace Reffy.Caches
{
    public class ArrayKeyCache<T, U>
    {
#if NET40 || NET45 || NET46 || NET472 || NET48 || NETCOREAPP3_1 || NET5_0
        private ConcurrentDictionary<T[], U> _cache
            = new ConcurrentDictionary<T[], U>(new ArrayEquallity());

        public bool TryGetValue(T[] key, out U value)
            => _cache.TryGetValue(key, out value);

        public bool TryAdd(T[] key, U value)
                => _cache.TryAdd(key, value);

        public U GetOrAdd(T[] key, U value)
            => _cache.GetOrAdd(key, value);

#else
        private Dictionary<T[], U> _cache
            = new Dictionary<T[], U>(new ArrayEquallity());

        public bool TryGetValue(T[] key, out U value)
            => _cache.TryGetValue(key, out value);

        public bool TryAdd(T[] key, U value)
        {
            if (_cache.TryGetValue(key, out U _))
                return false;

            lock (_cache)
            {
                _cache.Add(key, value);
            }
            return true;
        }

        public U GetOrAdd(T[] key, U value)
        {
            if (_cache.TryGetValue(key, out U v))
                return v;

            if (TryAdd(key, value))
                return value;

            throw new Exception("'value' add failed.");
        }
#endif
        private class ArrayEquallity : IEqualityComparer<T[]>
        {
            public bool Equals(T[] x, T[] y)
            {
                if (x.Length != y.Length)
                    return false;
                for (int i = 0; i < x.Length; ++i)
                {
                    if (!x[i].Equals(y[i]))
                        return false;
                }
                return true;
            }

            public int GetHashCode(T[] obj)
            {
                int returnValue = obj[0].GetHashCode();
                for (int i = 1; i < obj.Length; ++i)
                    returnValue ^= obj[i].GetHashCode();
                return returnValue;
            }
        }
    }
}
