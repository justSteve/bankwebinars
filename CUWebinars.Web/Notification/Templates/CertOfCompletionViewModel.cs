using CUWebinars.Business.Models;

namespace CUWebinars.Web.Notification.Templates
{
    public class CertOfCompletionViewModel
    {
        public Order Order { get; set; }
        public WebUser CurrentUser { get; set; }
    }
}