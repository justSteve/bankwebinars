using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class CheckoutOptionsViewModel
    {
        public DisplayOptionsViewModel DisplayOptionsViewModel { get; set; }
        public Order Order { get; set; }
        public Webinar Webinar { get; set; }
    }
}