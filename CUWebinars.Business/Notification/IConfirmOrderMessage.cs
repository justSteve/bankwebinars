using CUWebinars.Business.Core;

namespace CUWebinars.Business.Notification
{
    public interface IConfirmOrderMessage
    {
        string AddPasswordUrl { get; set; }
        string BaseUrl { get; set; }
        string ConfirmChangeEmailUrl { get; set; }
        string Details { get; set; }
        int idOrder { get; set; }
        OrderGenesis OrderGenesis { get; set; }
        string PersistedName { get; set; }
        bool UserCreatedInCart { get; set; }
        bool UserCreatedOnImport { get; set; }
    }
}
