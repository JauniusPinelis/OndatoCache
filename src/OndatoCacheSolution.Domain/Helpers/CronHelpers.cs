using System;

namespace OndatoCacheSolution.Domain.Helpers
{
    public static class CronHelpers
    {
        public static string ConvertToCronExpression(TimeSpan periodRecurrence)
        {
            if (periodRecurrence.Hours >= 1)
            {
                if (periodRecurrence.Seconds != 0) throw new ArgumentException("Seconds not allowed when hours are specified.");

                return ($"{periodRecurrence.Minutes} */{periodRecurrence.Hours} * * *");
            }

            if (periodRecurrence.Minutes > 1)
            {
                if (periodRecurrence.Seconds != 0) throw new ArgumentException("Seconds not allowed when minutes are specified.");

                return ($"*/{periodRecurrence.Minutes} * * * *");
            }

            return ($"*/1 * * * *");
        }
    }
}
