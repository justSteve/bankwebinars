using CUWebinars.Business.Models;

namespace CUWebinars.Business.Notification.Email
{
    public class AdditionalLocationOrderDetailsMessage : IAdditionalLocationOrderDetailsMessage
    {
        public string BaseUrl { get; set; }
        public string ConfirmChangeEmailUrl { get; set; }
        public string Details { get; set; }
        public int idOrder { get; set; }
        public Order Order { get; set; }
        public string NotifyAddress { get; set; }
        public string PersistedName { get; set; }
        public bool UserCreatedInCart { get; set; }
        public bool UserCreatedOnImport { get; set; } 
    }
}