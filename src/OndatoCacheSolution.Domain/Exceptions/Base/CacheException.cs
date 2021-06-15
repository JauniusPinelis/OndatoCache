using System;

namespace OndatoCacheSolution.Domain.Exceptions.Base
{
    public class CacheException : Exception
    {
        public CacheException(string message) : base(message)
        {

        }
    }
}
