using System.Globalization;
using System.Text.RegularExpressions;

namespace MadWin.Infrastructure.Convertors
{
    public static class ConvertDateTime
    {
        public static DateTime ToGreDateTime(this string pedate, bool shortDate = false)
        {
            // تبدیل اعداد فارسی به انگلیسی
            string pedateConverted = Regex.Replace(pedate, "[۰-۹]", x => ((char)(x.Value[0] - '۰' + '0')).ToString());

            // جدا کردن سال/ماه/روز
            var parts = pedateConverted.Split('/');
            if (parts.Length < 3)
                throw new FormatException("تاریخ فارسی وارد شده نامعتبر است.");

            int year = int.Parse(parts[0]);
            int month = int.Parse(parts[1]);
            int day = int.Parse(parts[2]);

            PersianCalendar pc = new PersianCalendar();

            if (!shortDate)
            {
                int hour = 0;
                int minute = 0;
                int second = 0;

                if (parts.Length >= 6) // اگر ساعت:دقیقه:ثانیه هم باشد
                {
                    hour = int.Parse(parts[3]);
                    minute = int.Parse(parts[4]);
                    second = int.Parse(parts[5]);
                }
                else
                {
                    hour = DateTime.Now.Hour;
                    minute = DateTime.Now.Minute;
                    second = DateTime.Now.Second;
                }

                return pc.ToDateTime(year, month, day, hour, minute, second, 0);
            }
            else
            {
                return pc.ToDateTime(year, month, day, 0, 0, 0, 0).Date;
            }
        }
    }
}
