using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class CertOfCompletionViewModel
    {
        public Order Order { get; set; }
        public WebUser CurrentUser { get; set; }
    }
}