using OndatoCacheSolution.Domain.Converters;
using OndatoCacheSolution.Domain.Enums;
using System;
using System.Text.Json.Serialization;

namespace OndatoCacheSolution.Domain.Configurations
{
    public class CacheConfiguration
    {
        [JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan DefaultExpirationPeriod { get; init; }

        [JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan MaxExpirationPeriod { get; init; }

        [JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan CleanupInterval { get; init; }

        public CacheType CacheType { get; set; }
    }
}
