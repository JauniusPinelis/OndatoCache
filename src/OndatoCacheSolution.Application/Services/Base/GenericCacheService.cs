using OndatoCacheSolution.Domain.Dtos;
using OndatoCacheSolution.Domain.Exceptions;
using OndatoCacheSolution.Domain.Exceptions.Base;
using OndatoCacheSolution.Domain.Factories;
using OndatoCacheSolution.Domain.Interfaces;
using OndatoCacheSolution.Domain.Validators;
using System;
using System.Linq;

namespace OndatoCacheSolution.Application.Services.Base
{
    public abstract class GenericCacheService<TKey, TValue>
    {
        protected readonly ICache<TKey, TValue> _cache;
        protected readonly CacheItemFactory<TKey, TValue> _cacheItemFactory;
        protected readonly CreateCacheItemValidator<TValue> _validator;

        protected GenericCacheService(ICache<TKey, TValue> cache, CacheItemFactory<TKey, TValue> cacheItemFactory, CreateCacheItemValidator<TValue> validator)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _cacheItemFactory = cacheItemFactory ?? throw new ArgumentNullException(nameof(cacheItemFactory));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public TValue Get(TKey key)
        {
            return _cache.Get(key);
        }

        public void Create(CreateCacheItemDto<TKey, TValue> itemDto)
        {
            var cacheItem = _cacheItemFactory.Build(itemDto);

            var validationResult = _validator.Validate(cacheItem);

            if (!validationResult.IsValid)
            {
                throw new CacheValidationException(validationResult.ToString());
            }

            _cache.Set(itemDto.Key, cacheItem);
        }

        public void Remove(TKey key)
        {
            _cache.Remove(key);
        }

        public void CleanExpired()
        {
            var keys = _cache.GetAllKeys().ToList(); //Load to memory

            foreach (var key in keys)
            {
                try
                {
                    _cache.CheckExpired(key);// Getting key refreshes the cache
                }
                catch (CacheException)
                {
                    //continue
                }
            }
        }
    }
}
