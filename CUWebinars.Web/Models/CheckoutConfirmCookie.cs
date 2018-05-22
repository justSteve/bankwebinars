using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class CheckoutConfirmCookieModel
    {
        public string InitialAffiliate { get; set; }
        public string InitialQueryString { get; set; }
        public string InitialValues { get; set; }
        //public Order Order { get; set; }

    }
}