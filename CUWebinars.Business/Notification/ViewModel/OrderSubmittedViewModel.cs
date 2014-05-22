using CUWebinars.Business.Models;

namespace CUWebinars.Business.Notification.ViewModel
{
    public class OrderSubmittedViewModel
    {
        public Order Order { get; set; }
        public string VerificationKey { get; set; }
    }
}