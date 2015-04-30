using System;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Core.Extensions
{
    public static class WebinarExtensions
    {
        public static bool CitrixJoinInfoAvailable(this Webinar source)
        {
            return source.Date.Subtract(TimeSpan.FromHours(1)) < DateTime.Now;
        }
    }
}