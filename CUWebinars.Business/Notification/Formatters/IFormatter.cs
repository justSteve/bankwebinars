
namespace CUWebinars.Business.Notification.Formatters
{
    public interface IFormatter
    {
        INotificationMessage Format<T>(T objectOfMessage);
    }
}
