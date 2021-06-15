namespace OndatoCacheSolution.Domain.Dtos
{
    public class CreateCacheItemDto<TKey, T>
    {
        public string Offset { get; set; }

        public TKey Key { get; set; }

        public T Value { get; set; }
    }
}
