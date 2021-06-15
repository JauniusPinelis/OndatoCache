using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OndatoCacheSolution.Infrastructure.Entities
{
    public class DbCacheValue
    {
        public int Id { get; set; }
        public string Value { get; set; }

        public string KeyId { get; set; }

        public DbCacheKey Key { get; set; }
    }
}
