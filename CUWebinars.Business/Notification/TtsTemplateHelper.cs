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

        public string DisplayDate(DateTime date)
        {
            return DateTimeHelper.FormatDate(date);
        }

        public string DisplayTime( DateTime time, USTimeZone timeZone,
            bool displayTimezone)
        {
            return "";
        }


        public string DisplayDateShort(DateTime date)
        {
            return DateTimeHelper.FormatDateShort(date);
        }

        //DateTime time
        //    , USTimeZone timeZone
        //    , bool displayTimezone
        //    ,decimal duration
        public string FormatTimeWithDuration(DateTime date, USTimeZone timeZone, bool displayTZ, decimal duration)
        {
            return DateTimeHelper.FormatTimeWithDuration(date, timeZone, displayTZ, duration);
        }
    }
}