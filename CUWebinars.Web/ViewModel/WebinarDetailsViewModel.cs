using System.Security.Claims;
using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Web.ViewModel
{
    public class WebinarDetailsViewModel
    {
        //public string btnWebinar { get; set; }
        //public string btnWhichState { get; set; }
        //public string btnHasOrder { get; set; }
        //public string stage_of_checkout { get; set; }

        //public Affiliate Affiliate { get; set; }
        public string CeuShort { get; set; }
        public string CeuStatement { get; set; }
        public CheckoutConfirmViewModel CheckoutConfirmViewModel { get; set; }
        public bool CheckoutInProcess { get; set; }
        public CheckoutOptionsViewModel CheckoutOptionsViewModel { get; set; }
        public string ConfirmationCaption { get; set; }
        public ClaimsIdentity Identity { get; set; }
        public string MessageOrderStatus { get; set; }
        public RegistrationSummaryViewModel RegistrationSummaryViewModel { get; set; }
        public IEnumerable<WebinarFile> WebinarFiles { get; set; }
        public Order Order { get; set; }
        public string SignUpCaption { get; set; }
        public string TimeFormatDisplay { get; set; }
        public USTimeZone TimeZone { get; set; }
        public IEnumerable<Topic> Topics { get; set; }
        public bool UserIsLoggedIn { get; set; }
        public int UserHasOpenOrder { get; set; }
        public int UserOwnsThisEvent { get; set; }

        public Webinar Webinar { get; set; }
        public WebUser WebUser { get; set; }
    }
}