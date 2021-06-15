using Microsoft.Extensions.DependencyInjection;
using OndatoCacheSolution.Domain.Caches;
using OndatoCacheSolution.Domain.Factories;
using OndatoCacheSolution.Domain.Interfaces;
using OndatoCacheSolution.Domain.Mappings;
using OndatoCacheSolution.Domain.Services;
using OndatoCacheSolution.Domain.Validators;

namespace OndatoCacheSolution.Domain
{
    public static class DependencyInjection
    {
        public static void ConfigureDomainServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(CacheItemFactory<,>));
            services.AddScoped(typeof(CreateCacheItemValidator<>));
            services.AddSingleton(typeof(MemoryCache<,>));

            services.AddSingleton<IDateTimeOffsetService, DateTimeOffsetService>();

            services.AddAutoMapper(typeof(MappingsProfile));
        }
    }
}
