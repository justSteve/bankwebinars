using CUWebinars.Business.Models;

namespace CUWebinars.Business.Notification.ViewModel
{
    public class OrderSubmittedAdditionalLocationViewModel
    {
        public string ConfirmChangeEmailUrl { get; set; }
        public Order Order { get; set; }
        public bool UserCreatedOnImport { get; set; }
        public string notifyAddress { get; set; }
    }
}