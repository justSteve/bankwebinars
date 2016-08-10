using System;
using System.Collections.Generic;
using System.Security.Claims;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Notification.ViewModel
{
    public class WebinarPromoViewModel
    {
        public string SubscriptionPackURL;
        public ClaimsIdentity Identity { get; set; }
        public string TemplateType { get; set; } // will the template be the 'per day' or  per week
        public Affiliate Affiliate { get; set; }
        public IList<Affiliate> Affiliates { get; set; }
        public string From { get; set; }
        public Webinar Webinar { get; set; }
        public DateTime SendDate { get; set; }
        public string EventBody { get; set; }
        public string FormattedDateTime { get; set; }
        public int[] ListOfWebinarsForWeekly { get; set; }
        public int[] ListOfWebinarsForUpcoming { get; set; }
        public string PresenterW_OutPic { get; set; }

        public USTimeZone TimeZone { get; set; }
        public string ListOfWebinarsUpcomingRendered { get; set; }
        public string BasePrice { get; set; }
    }
}
