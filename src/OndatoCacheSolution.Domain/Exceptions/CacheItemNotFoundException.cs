using OndatoCacheSolution.Domain.Exceptions.Base;

namespace OndatoCacheSolution.Domain.Exceptions
{
    public class CacheItemNotFoundException : CacheException
    {
        public CacheItemNotFoundException(string message) : base(message)
        {

        }
    }
}
