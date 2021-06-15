using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OndatoCacheSolution.Application.Services;
using OndatoCacheSolution.Domain.Configurations;
using OndatoCacheSolution.Domain.Helpers;
using System.Threading.Tasks;

namespace OndatoCacheSolution.Application.Middlewares
{
    public class CleanCacheTaskMiddleware
    {
        private readonly RequestDelegate _next;

        public CleanCacheTaskMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context, ObjectListCacheService cacheService,
            IOptions<CacheConfiguration> cacheConfiguration
            )
        {
            var cronExpression = CronHelpers.ConvertToCronExpression(cacheConfiguration.Value.CleanupInterval);
            RecurringJob.AddOrUpdate("CleanCache", () => cacheService.CleanExpired(), cronExpression);

            await _next.Invoke(context);
        }

    }
}
