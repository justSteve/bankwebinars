using CUWebinars.Business.Core;

namespace CUWebinars.Business.Notification.Email
{
    public class ConfirmOrderMessage : IConfirmOrderMessage
    {
        public string AddPasswordUrl { get; set; }
        public string BaseUrl { get; set; }
        public string ConfirmChangeEmailUrl { get; set; }
        public string Details { get; set; }
        public int idOrder { get; set; }
        public OrderGenesis OrderGenesis { get; set; }
        public string PersistedName { get; set; }
        public bool UserCreatedInCart { get; set; }
        public bool UserCreatedOnImport { get; set; }
    }
}
