using OndatoCacheSolution.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OndatoCacheSolution.Domain.Services
{
    public class DateTimeOffsetService : IDateTimeOffsetService
    {
        public DateTimeOffset Now()
        {
            return DateTimeOffset.Now;
        }
    }
}
