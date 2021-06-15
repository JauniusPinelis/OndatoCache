using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using OndatoCacheSolution.Domain.Dtos;
using OndatoCacheSolution.IntegrationTests.ApplicationFactories;
using OndatoCacheSolution.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace OndatoCacheSolution.IntegrationTests
{
    public class DbCacheControllerTests : IClassFixture<DbCacheApplicationFactory<Startup>>
    {
        private readonly DbCacheApplicationFactory<Startup> _factory;
        private readonly Fixture _fixture = new();


        public DbCacheControllerTests(DbCacheApplicationFactory<Startup> factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        [Fact]
        public async Task Create_CreatingCacheItem_GetsTheItem()
        {
            var client = _factory.CreateClient();

            var dto = _fixture.Build<CreateCacheItemDto<string, List<object>>>()
                .With(c => c.Value, _fixture.CreateMany<object>().ToList()).Create();

            var response = await client.PostAsJsonAsync("/Cache", dto);
            response.EnsureSuccessStatusCode();

            response = await client.GetAsync($"/Cache/{dto.Key}");
            response.EnsureSuccessStatusCode();

            var receivedStringValue = await response.Content.ReadAsStringAsync();
            var receivedValue = JsonConvert.DeserializeObject<List<object>>(receivedStringValue);

            receivedValue.Count.Should().Be(dto.Value.Count);
        }

        [Fact]
        public async Task Create_GivenExistingKey_ValueGetsOverwritten()
        {
            var client = _factory.CreateClient();
            const string key = "key";

            var dto = _fixture.Build<CreateCacheItemDto<string, List<object>>>()
                .With(c => c.Key, key)
                .With(c => c.Value, _fixture.CreateMany<object>(3).ToList()
                ).Create();

            var response = await client.PostAsJsonAsync("/Cache", dto);
            response.EnsureSuccessStatusCode();

            var newDto = _fixture.Build<CreateCacheItemDto<string, List<object>>>()
               .With(c => c.Key, key)
               .With(c => c.Value, _fixture.CreateMany<object>(5).ToList()
               ).Create();

            response = await client.PostAsJsonAsync("/Cache", newDto);
            response.EnsureSuccessStatusCode();

            response = await client.GetAsync($"/Cache/{newDto.Key}");
            response.EnsureSuccessStatusCode();

            var receivedStringValue = await response.Content.ReadAsStringAsync();
            var receivedValue = JsonConvert.DeserializeObject<List<object>>(receivedStringValue);

            receivedValue.Count.Should().Be(newDto.Value.Count);
        }

        [Fact]
        public async Task Delete_DeletingCreatedItem_ItemGetsDeleted()
        {
            var client = _factory.CreateClient();

            var dto = _fixture.Build<CreateCacheItemDto<string, List<object>>>()
                .With(c => c.Value, _fixture.CreateMany<object>().ToList()).Create();

            var response = await client.PostAsJsonAsync("/Cache", dto);
            response.EnsureSuccessStatusCode();

            await client.DeleteAsync($"/Cache/{dto.Key}");

            response = await client.GetAsync($"/Cache/{dto.Key}");
            response.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task Put_GivenExistingItem_DataGetsAppended()
        {
            var client = _factory.CreateClient();

            var dto = _fixture.Build<CreateCacheItemDto<string, List<object>>>()
                .With(c => c.Value, _fixture.CreateMany<object>().ToList()).Create();

            var response = await client.PostAsJsonAsync("/Cache", dto);
            response.EnsureSuccessStatusCode();

            response = await client.PutAsJsonAsync("/Cache", dto);
            response.EnsureSuccessStatusCode();

            response = await client.GetAsync($"/Cache/{dto.Key}");
            response.EnsureSuccessStatusCode();

            var receivedStringValue = await response.Content.ReadAsStringAsync();
            var receivedValue = JsonConvert.DeserializeObject<List<object>>(receivedStringValue);

            receivedValue.Count.Should().Be(dto.Value.Count * 2);
        }


        [Fact]
        public async Task Create_Given1MsTimeSpan_ValueGetsDeletedOnGet()
        {
            var client = _factory.CreateClient();

            var dto = _fixture.Build<CreateCacheItemDto<string, List<object>>>()
                .With(c => c.Key, "00:00:00.001")
                .With(c => c.Value, _fixture.CreateMany<object>().ToList())
                .With(c => c.Offset, "00:00:00.001")
                .Create();

            var response = await client.PostAsJsonAsync("/Cache", dto);
            response.EnsureSuccessStatusCode();

            await Task.Delay(100); //Need to wait couple ms to trigger deleted

            response = await client.GetAsync($"/Cache/{dto.Key}");
            response.StatusCode.Should().Be(404);

        }

        [Fact]
        public async Task Put_GivenNonExistingKey_ItemGetsCreated()
        {
            var client = _factory.CreateClient();

            var dto = _fixture.Build<CreateCacheItemDto<string, List<object>>>()
                .With(c => c.Key, "NonExisting")
                .With(c => c.Value, _fixture.CreateMany<object>().ToList()).Create();

            var response = await client.PostAsJsonAsync("/Cache", dto);
            response.EnsureSuccessStatusCode();

            response = await client.PutAsJsonAsync("/Cache", dto);
            response.EnsureSuccessStatusCode();

            response = await client.GetAsync($"/Cache/{dto.Key}");
            response.EnsureSuccessStatusCode();

            var receivedStringValue = await response.Content.ReadAsStringAsync();
            var receivedValue = JsonConvert.DeserializeObject<List<object>>(receivedStringValue);

            receivedValue.Count.Should().Be(dto.Value.Count * 2);
        }

        [Fact]
        public async Task Create_GivenInvalidOffset_ReturnsValidationError()
        {
            var client = _factory.CreateClient();

            var dto = _fixture.Build<CreateCacheItemDto<string, List<object>>>()
                .With(c => c.Offset, "05:00:00")
                .Create();

            var response = await client.PostAsJsonAsync("/Cache", dto);

            response.StatusCode.Should().Be(400);
        }
    }
}
