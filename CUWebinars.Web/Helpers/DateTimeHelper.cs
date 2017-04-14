using System;
using CUWebinars.Business.Core;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Helpers
{
    public static class DateTimeHelper
    {
        public static string FormatTime(DateTime time)
        {
            return FormatTime(time, USTimeZone.Central, false);
        }

        public static string FormatTime(DateTime time, USTimeZone timeZone, bool displayTimezone)
        {
            //This method assumes that the time is following Central time zone

            int timeZoneDifference;
            string timeZoneSuffix;
            switch (timeZone)
            {
                case USTimeZone.Pacific:
                    timeZoneDifference = -2;
                    timeZoneSuffix = AppConst.TIME_ZONE_PACIFIC;
                    break;
                case USTimeZone.Mountain:
                    timeZoneDifference = -1;
                    timeZoneSuffix = AppConst.TIME_ZONE_MOUNTAIN;
                    break;
                case USTimeZone.Central:
                    timeZoneDifference = 0;
                    timeZoneSuffix = AppConst.TIME_ZONE_CENTRAL;
                    break;
                case USTimeZone.Eastern:
                    timeZoneDifference = 1;
                    timeZoneSuffix = AppConst.TIME_ZONE_EASTERN;
                    break;
                default:
                    timeZoneDifference = 0;
                    timeZoneSuffix = AppConst.TIME_ZONE_CENTRAL;
                    break;
                    ;
            }

            DateTime timeToDisplay = time.AddHours(timeZoneDifference);

            if (displayTimezone == false)
            {
                timeZoneSuffix = String.Empty;
            }

            return timeToDisplay.ToString("h:mm tt").ToLower() + " " + timeZoneSuffix;
        }

        public static string FormatTimeWithDuration(DateTime time, USTimeZone timeZone, bool displayTimezone,
                                                    decimal duration)
        {
            //This function should build a string that diplays 
            //an event's time in beginning/ending format:
            //  
            int timeZoneDifference;
            string timeZoneSuffix;
            switch (timeZone)
            {
                case USTimeZone.Pacific:
                    timeZoneDifference = -2;
                    timeZoneSuffix = AppConst.TIME_ZONE_PACIFIC;
                    break;
                case USTimeZone.Mountain:
                    timeZoneDifference = -1;
                    timeZoneSuffix = AppConst.TIME_ZONE_MOUNTAIN;
                    break;
                case USTimeZone.Central:
                    timeZoneDifference = 0;
                    timeZoneSuffix = AppConst.TIME_ZONE_CENTRAL;
                    break;
                case USTimeZone.Eastern:
                    timeZoneDifference = 1;
                    timeZoneSuffix = AppConst.TIME_ZONE_EASTERN;
                    break;
                default:
                    timeZoneDifference = 0;
                    timeZoneSuffix = AppConst.TIME_ZONE_CENTRAL;
                    break;

            }



            DateTime timeToDisplay = time.AddHours(timeZoneDifference);

            DateTime durationToDisplay = time.AddHours(timeZoneDifference + (double)duration);
            return timeToDisplay.ToString("h:mm tt").ToLower() + " - " + durationToDisplay.ToString("h:mm tt").ToLower() +
                   " " + timeZoneSuffix;
        }

        public static string FormatDate(DateTime date)
        {
            return date.ToString("dddd, MMMM d") + GetOrdinalSuffix(date.Day) + ", " + date.ToString("yyyy");
        }

        public static string FormatDateShort(DateTime date)
        {

            return date.ToString("MM/dd/yy");

        }

        public static DateTime ToDateTime(this decimal value)
        {
            string[] parts = value.ToString().Split(new char[] { '.' });

            int hours = Convert.ToInt32(parts[0]);
            int minutes = Convert.ToInt32(parts[1]);

            if ((hours > 23) || (hours < 0))
            {
                throw new ArgumentOutOfRangeException("value",
                                                      "decimal value must be no greater than 23.59 and no less than 0");
            }
            if ((minutes > 59) || (minutes < 0))
            {
                throw new ArgumentOutOfRangeException("value",
                                                      "decimal value must be no greater than 23.59 and no less than 0");
            }
            DateTime d = new DateTime(1, 1, 1, hours, minutes, 0);
            return d;
        }

        private static string GetOrdinalSuffix(int number)
        {
            int condition1, condition2;
            condition1 = (number % 100);
            condition2 = (number % 10);

            if (condition1 == 11 || condition1 == 12 || condition1 == 13)
            {
                return "th";
            }

            switch (condition2)
            {
                case 1:
                    return "st";
                case 2:
                    return "nd";
                case 3:
                    return "rd";
                default:
                    return "th";
            }
        }

        public static DateTime GetDayStart(DateTime day)
        {
            return new DateTime(day.Year, day.Month, day.Day);
        }

        public static DateTime GetDayEnd(DateTime day)
        {
            return new DateTime(day.Year, day.Month, day.Day, 23, 59, 59);
        }

        public static DateTime GetStartOfCurrentWeek()
        {
            System.Globalization.CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
            DayOfWeek fdow = ci.DateTimeFormat.FirstDayOfWeek;
            return DateTime.Today.AddDays(-(DateTime.Today.DayOfWeek - fdow));
        }

        public static DateTime GetStartOfMonth(DateTime dayFromTheMonth)
        {
            return new DateTime(dayFromTheMonth.Year, dayFromTheMonth.Month, 1);
        }

        public static DateTime GetEndOfMonth(DateTime dayFromTheMonth)
        {
            return new DateTime(dayFromTheMonth.Year, dayFromTheMonth.AddMonths(1).Month, 1).AddDays(-1);
        }

        public static object ToDayWeekExpression(DateTime thisClaimExpiryDate)
        {
            throw new NotImplementedException();
        }

        public static DateTime GetDateOfNextDay(DateTime start, DayOfWeek day)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }

        public static DateTime? UtcNowAsCts
        {
            get
            {
                DateTime timeUtc = DateTime.UtcNow;
                return TimeZoneInfo.ConvertTimeFromUtc(timeUtc, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            }

        }

        public static string GetTimeInCentralStandardTime(DateTime time)
        {
            TimeZoneInfo centralStandardTime = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            DateTimeOffset timeInCST = TimeZoneInfo.ConvertTime(time, centralStandardTime);
            return timeInCST.ToString("yyyy-MM-dd hh:mm:ss tt\" GMT\"zzz");
        }
    }
}
