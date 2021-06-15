using OndatoCacheSolution.Domain.Exceptions;
using OndatoCacheSolution.Domain.Helpers;
using OndatoCacheSolution.Domain.Interfaces;
using OndatoCacheSolution.Domain.Models;
using OndatoCacheSolution.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OndatoCacheSolution.Infrastructure.Caches
{
    public class DbCache : ICache<string, IEnumerable<object>>
    {
        private readonly DataContext _dataContext;
        private readonly IDateTimeOffsetService _dateTimeOffsetService;

        public DbCache(DataContext dataContext, IDateTimeOffsetService dateTimeOffsetService)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _dateTimeOffsetService = dateTimeOffsetService ?? throw new ArgumentNullException(nameof(dateTimeOffsetService));
        }

        public void CheckExpired(string key)
        {
            var keyEntity = _dataContext.Keys.FirstOrDefault(k => k.Key == key);

            if (keyEntity == null)
            {
                throw new CacheItemNotFoundException($"{key} was not found in the cache");
            }

            if (_dateTimeOffsetService.Now() - keyEntity.LastRefreshed >= keyEntity.ExpiresAfter)
            {
                Remove(key);
                throw new CacheValidationException($"{key} was not found in the cache");
            }
        }

        public IEnumerable<object> Get(string key)
        {
            CheckExpired(key);

            var keyEntity = _dataContext.Keys.FirstOrDefault(k => k.Key == key);

            keyEntity.LastRefreshed = _dateTimeOffsetService.Now();

            _dataContext.SaveChanges();

            var unparsedValues = _dataContext.Values.Where(v => v.KeyId == key).Select(v => v.Value)
                .ToList();
            return unparsedValues.Select(v => SerializeHelpers.Deserialize(v));
        }

        public IEnumerable<string> GetAllKeys()
        {
            return _dataContext.Keys.Select(k => k.Key);
        }

        public void Remove(string key)
        {
            var values = _dataContext.Values.Where(v => v.KeyId == key).ToList();
            _dataContext.RemoveRange(values);

            var keyToDelete = _dataContext.Keys.FirstOrDefault(k => k.Key == key);
            _dataContext.Keys.Remove(keyToDelete);

            _dataContext.SaveChanges();
        }

        public void Set(string key, CacheItem<IEnumerable<object>> cacheItem)
        {
            Set(key, cacheItem.Value, cacheItem.ExpiresAfter);
        }

        public void Set(string key, IEnumerable<object> value, TimeSpan expiresAfter)
        {
            var keyEntity = _dataContext.Keys.FirstOrDefault(k => k.Key == key);

            if (keyEntity == null)
            {
                //Key not found - create a new one
                keyEntity = new DbCacheKey()
                {
                    Key = key,
                    LastRefreshed = _dateTimeOffsetService.Now(),
                    ExpiresAfter = expiresAfter
                };

                _dataContext.Add(keyEntity);
            }
            else
            {
                // just update the expiry
                keyEntity.ExpiresAfter = expiresAfter;
                _dataContext.Update(keyEntity);
            }

            //Remove existing values

            var existingValues = _dataContext.Values.Where(v => v.KeyId == key);
            _dataContext.RemoveRange(existingValues);

            var dbValues = value.Select(v => new DbCacheValue()
            {
                KeyId = key,
                Value = SerializeHelpers.Serialize(v)
            });

            _dataContext.AddRange(dbValues);
            _dataContext.SaveChanges();
        }
    }
}
