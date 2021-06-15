using Microsoft.Extensions.Options;
using OndatoCacheSolution.Domain.Configurations;
using OndatoCacheSolution.Domain.Dtos;
using OndatoCacheSolution.Domain.Models;
using System;

namespace OndatoCacheSolution.Domain.Factories
{
    public class CacheItemFactory<TKey, T>
    {
        private readonly CacheConfiguration _cacheConfiguration;

        public CacheItemFactory(IOptions<CacheConfiguration> cacheConfiguration)
        {
            _cacheConfiguration = cacheConfiguration.Value;
        }

        public CacheItem<T> Build(CreateCacheItemDto<TKey, T> dto)
        {
            TimeSpan offsetValue;

            try
            {
                offsetValue = TimeSpan.Parse(dto.Offset);
            }
            catch (Exception)
            {
                offsetValue = _cacheConfiguration.DefaultExpirationPeriod; //Set default value;
            }

            return new CacheItem<T>(dto.Value, offsetValue);
        }
    }
}
