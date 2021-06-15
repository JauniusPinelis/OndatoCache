using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using OndatoCacheSolution.Domain.Configurations;
using OndatoCacheSolution.Domain.Enums;
using OndatoCacheSolution.Infrastructure;
using OndatoCacheSolution.Infrastructure.Caches;
using System.Linq;

namespace OndatoCacheSolution.IntegrationTests.ApplicationFactories
{
    public class MemoryCacheApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
             d => d.ServiceType ==
                 typeof(DataContext));

                services.Remove(descriptor);

                descriptor = services.SingleOrDefault(
            d => d.ServiceType ==
                typeof(DbCache));

                services.Remove(descriptor);

                services.Configure<CacheConfiguration>(opts =>
                {
                    opts.CacheType = CacheType.Memory;
                });
            });


        }
    }
}
