
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Notification
{
    public interface IAdditionalLocationOrderDetailsMessage
    {
        string BaseUrl { get; set; }
        string ConfirmChangeEmailUrl { get; set; }
        string Details { get; set; }
        int idOrder { get; set; }
        Order Order { get; set; }
        string NotifyAddress { get; set; }
        string PersistedName { get; set; }
        bool UserCreatedInCart { get; set; }
        bool UserCreatedOnImport { get; set; }
    }
}