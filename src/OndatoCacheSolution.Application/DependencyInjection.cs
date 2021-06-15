using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OndatoCacheSolution.Application.Factories;
using OndatoCacheSolution.Application.Interfaces;
using OndatoCacheSolution.Application.Middlewares;
using OndatoCacheSolution.Application.Services;
using OndatoCacheSolution.Domain;
using OndatoCacheSolution.Domain.Configurations;
using OndatoCacheSolution.Infrastructure;

namespace OndatoCacheSolution.Application
{
    public static class DependencyInjection
    {
        public static void ConfigureApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureDomainServices();
            services.ConfigureInfrastructure(configuration);

            services.Configure<CacheConfiguration>(configuration.GetSection("CacheConfiguration"));


            services.AddScoped<ICacheFactory, CacheFactory>();
            services.AddScoped<ObjectListCacheService>();
        }

        public static void UseHangfireMiddleware(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard();
            app.UseMiddleware<CleanCacheTaskMiddleware>();
        }
    }
}
