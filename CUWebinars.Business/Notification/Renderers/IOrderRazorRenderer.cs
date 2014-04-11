using CUWebinars.Business.Notification.Events;

namespace CUWebinars.Business.Notification.Renderers
{
    public interface IOrderRazorRenderer<TOrder>
    {
        string ConstructMessage<T>(OrderSubmittedEvent<TOrder> orderSubmittedEvent,
            EnvironmentInformation applicationInformation,
            string templatedText,
            T objectOfMessage
            );
    }
}