using System;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Notification
{
    public class TtsTemplateHelper
    {
        public string SayHi()
        {
            return "hi!";
        }

        public static string DisplayDate(DateTime date)
        {
            return DateTimeHelper.FormatDate(date);
        }

        public static string DisplayTime( DateTime time, USTimeZone timeZone,
            bool displayTimezone)
        {
            return DateTimeHelper.FormatTime(time, timeZone, displayTimezone);
        }


        public static string DisplayDateShort( DateTime date)
        {
            return DateTimeHelper.FormatDateShort(date);
        }
    }
}