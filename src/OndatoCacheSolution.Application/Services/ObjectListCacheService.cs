using OndatoCacheSolution.Application.Interfaces;
using OndatoCacheSolution.Application.Services.Base;
using OndatoCacheSolution.Domain.Dtos;
using OndatoCacheSolution.Domain.Factories;
using OndatoCacheSolution.Domain.Validators;
using System.Collections.Generic;
using System.Linq;

namespace OndatoCacheSolution.Application.Services
{
    public class ObjectListCacheService : GenericCacheService<string, IEnumerable<object>>
    {
        public ObjectListCacheService(
            ICacheFactory cacheFactory,
            CacheItemFactory<string, IEnumerable<object>> cacheItemFactory,
            CreateCacheItemValidator<IEnumerable<object>> validator) : base(cacheFactory.Build(), cacheItemFactory, validator)
        {
        }

        public void Append(CreateCacheItemDto<string, IEnumerable<object>> itemDto)
        {
            var cacheItem = _cacheItemFactory.Build(itemDto);
            var cachedValue = _cache.Get(itemDto.Key);

            _cache.Set(itemDto.Key, cachedValue.Concat(cacheItem.Value), cacheItem.ExpiresAfter);
        }
    }
}
