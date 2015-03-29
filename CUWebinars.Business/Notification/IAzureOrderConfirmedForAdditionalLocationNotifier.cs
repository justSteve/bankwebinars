namespace CUWebinars.Business.Notification
{
    public interface IAzureOrderConfirmedForAdditionalLocationNotifier
    {
        void Notify(IAdditionalLocationOrderDetailsMessage additionalLocationOrderDetailsMessage);
    }
}