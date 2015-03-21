
namespace CUWebinars.Business.Notification
{
    public interface IOrderConfirmedNotificationDelivery
    {
        void Notify(IConfirmOrderMessage confirmOrderMessage);
    }
}
