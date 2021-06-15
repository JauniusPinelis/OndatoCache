using Microsoft.AspNetCore.Mvc;
using OndatoCacheSolution.Application.Services.Base;
using OndatoCacheSolution.Domain.Dtos;
using OndatoCacheSolution.Domain.Exceptions.Base;
using System;

namespace OndatoCacheSolution.WebApi.Controllers.Base
{
    [ApiController]
    [Route("[controller]")]
    public abstract class CacheControllerBase<TKey, TValue> : ControllerBase
    {
        private readonly GenericCacheService<TKey, TValue> _cacheService;

        public CacheControllerBase(GenericCacheService<TKey, TValue> cacheService)
        {
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        [HttpPost]
        public IActionResult Create(CreateCacheItemDto<TKey, TValue> cacheItemDto)
        {
            try
            {
                _cacheService.Create(cacheItemDto);
            }
            catch (CacheException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpGet("{key}")]
        public IActionResult Get(TKey key)
        {
            try
            {
                var value = _cacheService.Get(key);
                return Ok(value);
            }
            catch (CacheException ex)
            {
                return NotFound(ex.Message);
            }

        }

        [HttpDelete("{key}")]
        public IActionResult Remove(TKey key)
        {
            _cacheService.Remove(key);
            return NoContent();
        }
    }
}
