using System;

namespace OAuth.Web
{
    public static class DateTimeUtility
    {
        public static long Epoch(this DateTime d)
        {
            return (long)(d.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}