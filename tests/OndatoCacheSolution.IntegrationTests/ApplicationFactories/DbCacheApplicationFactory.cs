using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OndatoCacheSolution.Domain.Configurations;
using OndatoCacheSolution.Domain.Enums;
using OndatoCacheSolution.Infrastructure;
using System.Linq;

namespace OndatoCacheSolution.IntegrationTests.ApplicationFactories
{
    public class DbCacheApplicationFactory<TStartup>
     : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
               d => d.ServiceType ==
                   typeof(DbContextOptions<DataContext>));

                services.Remove(descriptor);

                services.AddDbContext<DataContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                services.Configure<CacheConfiguration>(opts =>
                {
                    opts.CacheType = CacheType.Database;
                });
            });
        }
    }
}
