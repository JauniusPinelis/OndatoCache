using FluentAssertions;
using OndatoCacheSolution.Domain.Converters;
using OndatoCacheSolution.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Cronos;

namespace OndatoCacheSolution.UnitTests.Helpers
{
    public class CronHelperTests
    {
        [Theory]
        [InlineData("05:00:00")]
        public static void ConvertToCronExpression_GivenCronExpression_ParsesIntoCorrectFormat(
            string input)
        {
            var timeSpanConverter = new TimeSpanConverter();
            var timeSpan = timeSpanConverter.Parse(input);
            
            var cronExpression = CronHelpers.ConvertToCronExpression(timeSpan);

            // If Parse fails this will throw exception and fail the tests
            CronExpression.Parse(cronExpression);
        }
    }
}
