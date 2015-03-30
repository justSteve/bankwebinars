namespace CUWebinars.Business.Notification
{
    public interface IOrderConfirmedForAdditionalLocationDelivery
    {
        void Notify(IAdditionalLocationOrderDetailsMessage additionalLocationOrderDetailsMessage);
    }
}