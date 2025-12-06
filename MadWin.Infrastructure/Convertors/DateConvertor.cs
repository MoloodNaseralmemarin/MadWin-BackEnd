using System;
using System.Globalization;

namespace Shop2City.Core.Convertors
{
    public static class DateConvertor
    {
        public static string ToShamsi(this DateTime value)
        {
            PersianCalendar pc = new PersianCalendar();
            return pc.GetYear(value) + "/" + pc.GetMonth(value).ToString("00") + "/" +
                   pc.GetDayOfMonth(value).ToString("00");
        }

            public static DateTime? ConvertPersianToGregorian(string? persianDate)
            {
                if (string.IsNullOrWhiteSpace(persianDate))
                    return null;

                // تبدیل اعداد فارسی و عربی به انگلیسی
                persianDate = ConvertToEnglishNumbers(persianDate);

                var parts = persianDate.Split('/', '-');
                if (parts.Length != 3)
                    return null;

                int year = int.Parse(parts[0]);
                int month = int.Parse(parts[1]);
                int day = int.Parse(parts[2]);

                var pc = new PersianCalendar();
                return pc.ToDateTime(year, month, day, 0, 0, 0, 0);
            }

            private static string ConvertToEnglishNumbers(string input)
            {
                if (string.IsNullOrEmpty(input))
                    return input;

                return input
                    .Replace("۰", "0").Replace("١", "1").Replace("۱", "1")
                    .Replace("٢", "2").Replace("۲", "2")
                    .Replace("٣", "3").Replace("۳", "3")
                    .Replace("٤", "4").Replace("۴", "4")
                    .Replace("٥", "5").Replace("۵", "5")
                    .Replace("٦", "6").Replace("۶", "6")
                    .Replace("٧", "7").Replace("۷", "7")
                    .Replace("٨", "8").Replace("۸", "8")
                    .Replace("٩", "9").Replace("۹", "9");
            }
        }
    }

