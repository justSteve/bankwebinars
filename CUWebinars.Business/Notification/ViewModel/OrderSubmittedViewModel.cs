using CUWebinars.Business.Models;

namespace CUWebinars.Business.Notification.ViewModel
{
    public class OrderSubmittedViewModel
    {
        public string ConfirmChangeEmailUrl { get; set; }
        public Order Order { get; set; }
        public string Password { get; set; }
        public bool UserCreatedOnImport { get; set; }
    }
}