using Microsoft.AspNetCore.Mvc;
using OndatoCacheSolution.Application.Services;
using OndatoCacheSolution.Domain.Dtos;
using OndatoCacheSolution.WebApi.Controllers.Base;
using System.Collections.Generic;

namespace OndatoCacheSolution.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CacheController : CacheControllerBase<string, IEnumerable<object>>
    {
        private readonly ObjectListCacheService _concreteCacheService;
        public CacheController(
            ObjectListCacheService concreteCacheService) : base(concreteCacheService)
        {
            _concreteCacheService = concreteCacheService;
        }

        [HttpPut]
        public IActionResult Append(CreateCacheItemDto<string, IEnumerable<object>> dto)
        {
            _concreteCacheService.Append(dto);

            return NoContent();
        }
    }
}
