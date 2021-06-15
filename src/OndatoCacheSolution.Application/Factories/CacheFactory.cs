using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OndatoCacheSolution.Application.Interfaces;
using OndatoCacheSolution.Domain.Caches;
using OndatoCacheSolution.Domain.Configurations;
using OndatoCacheSolution.Domain.Enums;
using OndatoCacheSolution.Domain.Interfaces;
using OndatoCacheSolution.Infrastructure.Caches;
using System;
using System.Collections.Generic;

namespace OndatoCacheSolution.Application.Factories
{
    public class CacheFactory : ICacheFactory
    {
        private readonly CacheConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public CacheFactory(IOptions<CacheConfiguration> configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration.Value;
            _serviceProvider = serviceProvider;
        }

        public ICache<string, IEnumerable<object>> Build()
        {
            switch (_configuration.CacheType)
            {
                case CacheType.Memory:
                    return _serviceProvider.GetRequiredService<MemoryCache<string, IEnumerable<object>>>();
                case CacheType.Database:
                    return _serviceProvider.GetRequiredService<DbCache>();
                default:
                    throw new Exception("Could not find the right cache");
            }
        }
    }
}
