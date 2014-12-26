using CUWebinars.Business.Models;

namespace CUWebinars.Business.Notification.ViewModel
{
    public class OrderSubmittedViewModel
    {
        public string AddPasswordUrl { get; set; }
        public string ConfirmChangeEmailUrl { get; set; }
        public Order Order { get; set; }
        public bool UserCreatedInCart { get; set; }
        public bool UserCreatedOnImport { get; set; }
    }
}