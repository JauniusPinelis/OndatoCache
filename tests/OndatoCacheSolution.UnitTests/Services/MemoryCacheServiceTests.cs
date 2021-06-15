using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using OndatoCacheSolution.Application.Interfaces;
using OndatoCacheSolution.Application.Services;
using OndatoCacheSolution.Domain.Caches;
using OndatoCacheSolution.Domain.Configurations;
using OndatoCacheSolution.Domain.Dtos;
using OndatoCacheSolution.Domain.Enums;
using OndatoCacheSolution.Domain.Factories;
using OndatoCacheSolution.Domain.Services;
using OndatoCacheSolution.Domain.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OndatoCacheSolution.UnitTests.Services
{
    public class MemoryCacheServiceTests
    {
        [Fact]
        public async Task CleanExpired_Given1ExpiredCacheRecord_CacheAfterCleanShouldBeEmpty()
        {
            var cacheSettings = new CacheConfiguration()
            {
                CleanupInterval = TimeSpan.FromMinutes(5),
                DefaultExpirationPeriod = TimeSpan.FromMilliseconds(1),
                MaxExpirationPeriod = TimeSpan.FromMinutes(5),
                CacheType = CacheType.Memory
            };


            // Resolve services from the IServiceProvider and pass it along

            var dateTimeOffsetService = new DateTimeOffsetService();
            var genericCache = new MemoryCache<string, IEnumerable<object>>(dateTimeOffsetService);
            var cacheItemFactory = new CacheItemFactory<string, IEnumerable<object>>(Options.Create(cacheSettings));


            var mockCacheFactory = new Mock<ICacheFactory>();
            mockCacheFactory.Setup(m => m.Build()).Returns(genericCache);


            var validator = new CreateCacheItemValidator<IEnumerable<object>>(Options.Create(cacheSettings));

            var cacheService = new ObjectListCacheService(mockCacheFactory.Object, cacheItemFactory, validator);

            var createCacheItemDTo = new CreateCacheItemDto<string, IEnumerable<object>>()
            {
                Key = "expired"
            };
            cacheService.Create(createCacheItemDTo);
            await Task.Delay(100);

            cacheService.CleanExpired();
            genericCache.GetAllKeys().Count().Should().Be(0);
        }


        [Fact]
        public static void CleanExpired_GivenNotExpiredCacheRecord_CacheShouldNotBeEmpty()
        {
            var cacheSettings = new CacheConfiguration()
            {
                CleanupInterval = TimeSpan.FromMinutes(5),
                DefaultExpirationPeriod = TimeSpan.FromMinutes(5),
                MaxExpirationPeriod = TimeSpan.FromMinutes(5),
            };
            var dateTimeOffsetService = new DateTimeOffsetService();
            var genericCache = new MemoryCache<string, IEnumerable<object>>(dateTimeOffsetService);

            var mockCacheFactory = new Mock<ICacheFactory>();
            mockCacheFactory.Setup(m => m.Build()).Returns(genericCache);


            var cacheItemFactory = new CacheItemFactory<string, IEnumerable<object>>(Options.Create(cacheSettings));
            var validator = new CreateCacheItemValidator<IEnumerable<object>>(Options.Create(cacheSettings));
            var cacheService = new ObjectListCacheService(mockCacheFactory.Object, cacheItemFactory, validator);

            var createCacheItemDTo = new CreateCacheItemDto<string, IEnumerable<object>>()
            {
                Key = "expired"
            };
            cacheService.Create(createCacheItemDTo);
            Task.Delay(1);

            cacheService.CleanExpired();

            genericCache.GetAllKeys().Should().NotBeEmpty();
        }
    }
}
