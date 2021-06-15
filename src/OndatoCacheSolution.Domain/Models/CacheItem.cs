using System;

namespace OndatoCacheSolution.Domain.Models
{
    public class CacheItem<T>
    {
        public T Value { get; init; }
        internal DateTimeOffset LastRefreshed { get; set; } = DateTimeOffset.Now;
        public TimeSpan ExpiresAfter { get; init; }

        public CacheItem()
        {

        }

        public CacheItem(T value, TimeSpan expiresAfter)
        {
            Value = value;
            ExpiresAfter = expiresAfter;
        }
    }
}
