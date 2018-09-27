using System;
using System.Collections.Generic;
using System.Linq;

namespace Cryptopia.API.Cache
{
    public class BasicCache<T,U> : ICache<T,U> where T : IComparable<T>
    {
        private readonly object _sync = new object();
        private readonly Dictionary<T, U> _memoryCache = new Dictionary<T,U>();
       
        public IEnumerable<U> GetAll()
        {
            return _memoryCache.Values.AsEnumerable();
        }

        public IEnumerable<U> GetAll(Func<U, bool> exp)
        {
            return _memoryCache.Values.Where(exp);
        }

        public IEnumerable<U> GetAll(Func<T, bool> exp)
        {
            return _memoryCache.Keys.Where(exp).Select(key => _memoryCache[key]);
        }

        public U Get(T key)
        {
            return _memoryCache[key];
        }

        public U Get(Func<U, bool> exp)
        {
            return _memoryCache.Values.First(exp);
        }

        public U Get(Func<T, bool> exp)
        {
            return _memoryCache[_memoryCache.Keys.First(exp)];
        }

        public U GetOrDefault(T key)
        {
            return Exists(key) ? _memoryCache[key] : default(U);
        }

        public U GetOrDefault(Func<U, bool> exp)
        {
            return _memoryCache.Values.FirstOrDefault(exp);
        }

        public U GetOrDefault(Func<T, bool> exp)
        {
            var key = _memoryCache.Keys.FirstOrDefault();
            return key != null ? _memoryCache[key] : default(U);
        }

        public void Remove(T obj)
        {
            lock (_sync)
            {
                _memoryCache.Remove(obj);
            }
        }

        public void Add(T key, U obj)
        {
            lock (_sync)
            {
                _memoryCache.Add(key,obj);
            }
        }

        public void AddOrUpdate(T key, U obj)
        {
            if (Exists(key))
            {
                Remove(key);
            }
            Add(key,obj);
        }

        public bool Exists(T obj)
        {
            return _memoryCache.ContainsKey(obj);
        }
    }
}
