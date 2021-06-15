using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OndatoCacheSolution.Infrastructure.Caches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OndatoCacheSolution.Infrastructure
{
    public static class DependencyInjection
    {
        public static void ConfigureInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            ConfigureDbContext(services, configuration);
            ConfigureHangFire(services, configuration);

            services.AddScoped<DbCache>();
        } 

        private static void ConfigureHangFire(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(c => c
           .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
           .UseSimpleAssemblyNameTypeSerializer()
           .UseRecommendedSerializerSettings()
           .UseMemoryStorage());

            services.AddHangfireServer();
        }


        private static void ConfigureDbContext(IServiceCollection services, IConfiguration configuration)
        {            services.AddDbContext<DataContext>(c => c.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));
        }

        
    }
}
