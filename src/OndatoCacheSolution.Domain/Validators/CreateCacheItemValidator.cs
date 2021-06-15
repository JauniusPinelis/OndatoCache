using FluentValidation;
using Microsoft.Extensions.Options;
using OndatoCacheSolution.Domain.Configurations;
using OndatoCacheSolution.Domain.Models;

namespace OndatoCacheSolution.Domain.Validators
{
    public class CreateCacheItemValidator<T> : AbstractValidator<CacheItem<T>>
    {

        public CreateCacheItemValidator(IOptions<CacheConfiguration> cacheSettings)
        {

            RuleFor(x => x.ExpiresAfter)
                .LessThanOrEqualTo(cacheSettings.Value.MaxExpirationPeriod);
        }
    }
}
