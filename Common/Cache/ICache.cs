using System;
using System.Collections.Generic;

namespace Cryptopia.API.Cache
{
    interface ICache<T, U>
     where T : IComparable<T>
    {
        void Add(T key, U obj);
        void AddOrUpdate(T key, U obj);
        bool Exists(T obj);
        U Get(Func<T, bool> exp);
        U Get(Func<U, bool> exp);
        U Get(T key);
        IEnumerable<U> GetAll(Func<T, bool> exp);
        IEnumerable<U> GetAll(Func<U, bool> exp);
        U GetOrDefault(Func<T, bool> exp);
        U GetOrDefault(Func<U, bool> exp);
        U GetOrDefault(T key);
        void Remove(T obj);
    }
}
