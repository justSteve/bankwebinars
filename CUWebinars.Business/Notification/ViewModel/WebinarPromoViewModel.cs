using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Notification.ViewModel
{
    public class WebinarPromoViewModel
    {
        public ClaimsIdentity Identity { get; set; }
        public string TemplateType { get; set; } // will the template be the 'per day' or  per week
        public Affiliate Affiliate { get; set; }
        public IList<Affiliate> Affiliates { get; set; }
        public string From { get; set; }
        public Webinar Webinar { get; set; }
        public IList<Webinar> Webinars { get; set; }
        public DateTime SendDate { get; set; }
        public string EventBody { get; set; }
        public string UpcomingListing { get; set; }

        public USTimeZone TimeZone { get; set; }
    }
}
