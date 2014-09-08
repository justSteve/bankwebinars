using System;
using System.Configuration;
using System.Diagnostics;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Core.Tracing;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Notification
{
    public class TtsTemplateHelper
    {
        public string FormatDate(DateTime date)
        {
            return DateTimeHelper.FormatDate(date);
        }
        public string Tenant()
        {
            return ConfigurationManager.AppSettings["Tenant"];

        }
        public string TenantLogo()
        {
            return ConfigurationManager.AppSettings["TenantLogo"];

        }

        public string TenantEmail()
        {
            return ConfigurationManager.AppSettings["TenantEmail"];

        }

        public string TenantDomain()
        {
            return ConfigurationManager.AppSettings["TenantDomain"];

        }

        public string TenantURL()
        {
            return ConfigurationManager.AppSettings["TenantURL"];

        }
        public string TenantPrefix()
        {
            return ConfigurationManager.AppSettings["TenantPrefix"];
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
            //Tracer.Verbose("Formatting time with duration.");

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