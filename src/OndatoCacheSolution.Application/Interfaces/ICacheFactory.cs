using OndatoCacheSolution.Domain.Interfaces;
using System.Collections.Generic;

namespace OndatoCacheSolution.Application.Interfaces
{
    public interface ICacheFactory
    {
        ICache<string, IEnumerable<object>> Build();
    }
}