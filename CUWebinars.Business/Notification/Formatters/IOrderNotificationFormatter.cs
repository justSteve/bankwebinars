namespace CUWebinars.Business.Notification.Formatters
{
    public interface IOrderNotificationFormatter<T>
    {
        INotificationMessage Format<TBody>(Events.OrderSubmittedEvent<T> orderSubmittedEvent, TBody objectOfMessage);
    }
}