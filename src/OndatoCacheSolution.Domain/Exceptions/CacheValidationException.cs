using OndatoCacheSolution.Domain.Exceptions.Base;

namespace OndatoCacheSolution.Domain.Exceptions
{
    public class CacheValidationException : CacheException
    {
        public CacheValidationException(string message) : base(message)
        {

        }
    }
}
