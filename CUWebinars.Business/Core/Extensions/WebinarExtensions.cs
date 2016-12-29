using System;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Core.Extensions
{
    public static class WebinarExtensions
    {
        public static bool CitrixJoinInfoAvailable(this Webinar source)
        {
            if (source.Status == WebinarStatus.Active)
                return true;
            return false;
        }
    }
}