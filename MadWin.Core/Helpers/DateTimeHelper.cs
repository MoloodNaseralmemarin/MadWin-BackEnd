using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadWin.Core.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime NowIran()
        {
            TimeZoneInfo iran = TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, iran);
        }
    }

}
