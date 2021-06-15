using OndatoCacheSolution.Domain.Exceptions;
using OndatoCacheSolution.Domain.Interfaces;
using OndatoCacheSolution.Domain.Models;
using System;
using System.Collections.Generic;

namespace OndatoCacheSolution.Domain.Caches
{
    public class MemoryCache<TKey, TValue> : ICache<TKey, TValue>
    {
        protected readonly IDateTimeOffsetService _dateTimeOffsetService;
        protected readonly Dictionary<TKey, CacheItem<TValue>> _cache = new();

        public MemoryCache(IDateTimeOffsetService dateTimeOffsetService)
        {
            _dateTimeOffsetService = dateTimeOffsetService ?? throw new ArgumentNullException(nameof(dateTimeOffsetService));
        }

        public virtual void Set(TKey key, TValue value, TimeSpan expiresAfter)
        {
            Set(key, new CacheItem<TValue>(value, expiresAfter));
        }

        public void Set(TKey key, CacheItem<TValue> cacheItem)
        {
            cacheItem.LastRefreshed = _dateTimeOffsetService.Now();
            _cache[key] = cacheItem;
        }

        public void Remove(TKey key)
        {
            _cache.Remove(key);
        }

        public TValue Get(TKey key)
        {
            CheckExpired(key);
            var cached = _cache[key];

            cached.LastRefreshed = _dateTimeOffsetService.Now();
            return cached.Value;
        }

        public IEnumerable<TKey> GetAllKeys()
        {
            return _cache.Keys;
        }

        public void CheckExpired(TKey key)
        {
            if (!_cache.ContainsKey(key))
            {
                throw new CacheItemNotFoundException($"{key} was not found in the cache");
            }

            var cached = _cache[key];
            if (_dateTimeOffsetService.Now() - cached.LastRefreshed >= cached.ExpiresAfter)
            {
                _cache.Remove(key);
                throw new CacheValidationException($"{key} was not found in the cache");
            }
        }
    }
}
