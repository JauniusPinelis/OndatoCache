using OndatoCacheSolution.Domain.Models;
using System;
using System.Collections.Generic;

namespace OndatoCacheSolution.Domain.Interfaces
{
    public interface ICache<TKey, TValue>
    {
        TValue Get(TKey key);
        IEnumerable<TKey> GetAllKeys();
        void Remove(TKey key);
        void Set(TKey key, CacheItem<TValue> cacheItem);
        void Set(TKey key, TValue value, TimeSpan expiresAfter);

        void CheckExpired(TKey key);


    }
}