using System;
using System.Collections.Generic;
using System.Security.Claims;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Notification.ViewModel
{
    public class WebinarPromoViewModel
    {
        public string SubscriptionPackURL;
        public string CEUValue;
        public ClaimsIdentity Identity { get; set; }
        public string TemplateType { get; set; } // will the template be the 'per day' or  per week
        public Affiliate Affiliate { get; set; }
        public IList<Affiliate> Affiliates { get; set; }
        public string From { get; set; }
        public Webinar Webinar { get; set; }
        public DateTime SendDate { get; set; }
        public string EventBody { get; set; }
        public string TimeFormatDisplay { get; set; }
        public string sListOfWebinarsForWeekly { get; set; }
        public int[] ListOfWebinarsForUpcoming { get; set; }
        public string PresenterW_OutPic { get; set; }

        public USTimeZone TimeZone { get; set; }
        public string ListOfWebinarsUpcomingRendered { get; set; }
        public string BasePrice { get; set; }
        public string Subject { get; set; }
        public string BodyLeft { get; set; }
        public string BodyRight { get; set; }
    }
}
