using CUWebinars.Business.Models;
using CUWebinars.Web.ViewModel;

namespace CUWebinars.Web.Models
{
    public class ExpressCheckoutPostBackModel
    {

        public string submission_id { get; set; }
        public string formID { get; set; }
        public string ip { get; set; }
        public string webinartitle12 { get; set; }
        public string email5 { get; set; }
        public string registrationtype { get; set; }
        public string title { get; set; }
        public string institution { get; set; }
        public string discountcode20 { get; set; }
        public string comments { get; set; }
        public string q_webinarid18 { get; set; }
        public string orderid { get; set; }
        public string affiliateid15 { get; set; }

        //
        public string UserIsConfirmed { get; set; }
        public string OrderIsConfirmed { get; set; }
        public Order Order { get; set; }
        public DisplayRowPriceViewModel DisplayRowPriceViewModel { get; set; }
    }
}