
namespace CUWebinars.Business.Notification
{
    public interface IOrderConfirmedForAdditionalLocationNotificationDelivery
    {
        void Notify(IAdditionalLocationOrderDetailsMessage confirmOrderMessage);
    }
}
