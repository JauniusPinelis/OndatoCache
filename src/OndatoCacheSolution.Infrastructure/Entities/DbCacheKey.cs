using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OndatoCacheSolution.Infrastructure.Entities
{
    public class DbCacheKey
    {
        public string Key { get; set; }
        public DateTimeOffset LastRefreshed { get; set; } = DateTimeOffset.Now;
        public TimeSpan ExpiresAfter { get; set; }

        public IEnumerable<DbCacheValue> Values { get; set; }
    }
}
