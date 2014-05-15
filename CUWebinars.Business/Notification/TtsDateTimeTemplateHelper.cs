using System;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Notification
{
    public class TtsDateTimeTemplateHelper
    {
        public string FormatDate(DateTime date)
        {
            return DateTimeHelper.FormatDate(date);
        }


        public string FormatDateShort(DateTime date)
        {
            return DateTimeHelper.FormatDateShort(date);
        }

        public string FormatTime(DateTime time)
        {
            return DateTimeHelper.FormatTime(time);
        }

        public string FormatTime(DateTime time, USTimeZone timeZone, bool displayTimezone)
        {
            return DateTimeHelper.FormatTime(time, timeZone, displayTimezone);
        }

        public string FormatTimeWithDuration(DateTime time, USTimeZone timeZone, bool displayTimezone, decimal duration)
        {
            return DateTimeHelper.FormatTimeWithDuration(time, timeZone, displayTimezone, duration);
        }

        public DateTime GetDayStart(DateTime day)
        {
            return DateTimeHelper.GetDayStart(day);
        }

        public DateTime GetDayEnd(DateTime day)
        {
            return DateTimeHelper.GetDayEnd(day);
        }

        public DateTime GetStartOfCurrentWeek()
        {
            return DateTimeHelper.GetStartOfCurrentWeek();
        }

        public DateTime GetEndOfMonth(DateTime dayFromTheMonth)
        {
            return DateTimeHelper.GetEndOfMonth(dayFromTheMonth);
        }

        public DateTime GetStartOfMonth(DateTime dayFromTheMonth)
        {
            return DateTimeHelper.GetStartOfMonth(dayFromTheMonth);
        }

        public DateTime ToDateTime(decimal value)
        {
            return DateTimeHelper.ToDateTime(value);
        }

     }
}