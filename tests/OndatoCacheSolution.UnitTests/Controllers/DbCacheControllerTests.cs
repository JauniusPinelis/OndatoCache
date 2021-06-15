using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using OndatoCacheSolution.Application.Interfaces;
using OndatoCacheSolution.Application.Services;
using OndatoCacheSolution.Domain.Configurations;
using OndatoCacheSolution.Domain.Dtos;
using OndatoCacheSolution.Domain.Factories;
using OndatoCacheSolution.Domain.Interfaces;
using OndatoCacheSolution.Domain.Validators;
using OndatoCacheSolution.Infrastructure;
using OndatoCacheSolution.Infrastructure.Caches;
using OndatoCacheSolution.WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace OndatoCacheSolution.UnitTests.Controllers
{
    public class DbCacheControllerTests
    {
        private ObjectListCacheService _cacheService;
        private Mock<IDateTimeOffsetService> _mockDateTimeOffsetService;
        private DataContext _dataContext;

        private readonly Fixture _fixture = new Fixture();

        private CacheController SetupCacheController()
        {
            var cacheSettings = Options.Create(new CacheConfiguration()
            {
                DefaultExpirationPeriod = TimeSpan.FromMinutes(5),
                MaxExpirationPeriod = TimeSpan.FromMinutes(5),
                CacheType = Domain.Enums.CacheType.Database
            });

            var validator = new CreateCacheItemValidator<IEnumerable<object>>(cacheSettings);

            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dataContext = new DataContext(options);

            _mockDateTimeOffsetService = new Mock<IDateTimeOffsetService>();
            _mockDateTimeOffsetService.Setup(m => m.Now()).Returns(DateTimeOffset.Now);

            var dbCache = new DbCache(_dataContext, _mockDateTimeOffsetService.Object);

            var cacheFactory = new Mock<ICacheFactory>();
            cacheFactory.Setup(m => m.Build()).Returns(dbCache);

            var cacheItemFactory = new CacheItemFactory<string, IEnumerable<object>>(cacheSettings);
            _cacheService = new ObjectListCacheService(cacheFactory.Object, cacheItemFactory, validator);
            return new CacheController(_cacheService);
        }

        [Fact]
        public void TestSetup_GivenCreateAndGet_ControllerDoesNotCrash()
        {

            var cacheController = SetupCacheController();

            var dto = _fixture.Create<CreateCacheItemDto<string, IEnumerable<object>>>();

            cacheController.Create(dto);

            var result = cacheController.Get(dto.Key) as OkObjectResult;

            var receivedDto = result.Value as IEnumerable<object>;

            receivedDto.Count().Should().Be(dto.Value.Count());
        }

        [Fact]
        public void Get_GivenOutdatedItem_GetReturns404()
        {

            var cacheController = SetupCacheController();

            var dto = _fixture.Create<CreateCacheItemDto<string, IEnumerable<object>>>();

            _mockDateTimeOffsetService.Setup(m => m.Now()).Returns(DateTimeOffset.MinValue);

            cacheController.Create(dto);

            _mockDateTimeOffsetService.Setup(m => m.Now()).Returns(DateTimeOffset.Now);

            var result = cacheController.Get(dto.Key) as NotFoundObjectResult;

            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public void CleanServiceTest_RanCleanCache_DatabaseShouldBeEmpty()
        {
            var cacheController = SetupCacheController();

            var dto = _fixture.Create<CreateCacheItemDto<string, IEnumerable<object>>>();

            _mockDateTimeOffsetService.Setup(m => m.Now()).Returns(DateTimeOffset.MinValue);

            cacheController.Create(dto);

            _mockDateTimeOffsetService.Setup(m => m.Now()).Returns(DateTimeOffset.Now);

            _cacheService.CleanExpired();

            _dataContext.Keys.Any().Should().BeFalse();
        }

        [Fact]
        public void CleanServiceTest_RanCleanCache_ShouldRemoveOutdatedItems()
        {
            var cacheController = SetupCacheController();

            var outdatedDto = _fixture.Create<CreateCacheItemDto<string, IEnumerable<object>>>();

            _mockDateTimeOffsetService.Setup(m => m.Now()).Returns(DateTimeOffset.MinValue);

            cacheController.Create(outdatedDto);

            _mockDateTimeOffsetService.Setup(m => m.Now()).Returns(DateTimeOffset.Now);

            var freshDto = _fixture.Create<CreateCacheItemDto<string, IEnumerable<object>>>();

            cacheController.Create(freshDto);

            _cacheService.CleanExpired();

            _dataContext.Keys.Count().Should().Be(1);
            _dataContext.Keys.First().Key.Should().Be(freshDto.Key);
        }

        [Fact]
        public void CleanServiceTest_GetRefreshesExpiry_RefreshedItemDoesnotGetRemoved()
        {
            var cacheController = SetupCacheController();


            var dto = _fixture.Build<CreateCacheItemDto<string, IEnumerable<object>>>()
                .With(c => c.Offset, "00:04:00").Create();

            _mockDateTimeOffsetService.Setup(m => m.Now()).Returns(DateTimeOffset.Now);

            var result = cacheController.Create(dto);

            _cacheService.CleanExpired();

            _mockDateTimeOffsetService.Setup(m => m.Now()).Returns(DateTimeOffset.Now.AddMinutes(3));

            cacheController.Get(dto.Key);

            _mockDateTimeOffsetService.Setup(m => m.Now()).Returns(DateTimeOffset.Now.AddMinutes(3));

            cacheController.Get(dto.Key);

            _cacheService.CleanExpired();

            _dataContext.Keys.Count().Should().Be(1);
            _dataContext.Keys.First().Key.Should().Be(dto.Key);
        }
    }
}
